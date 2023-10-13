using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Kraken.DevCon
{
    public interface IConsoleVariable
    {
        object value { get; set; }
    }

    /// <summary>
    /// Represents a Console Variable. You need to parse it as you see fit.
    /// Also invokes an event when value is changed. Modify dependent systems using that or perform verifications on that value like limiting it to a range.
    /// </summary>
    [System.Serializable]
    public class ConsoleVariable<T> : IConsoleVariable
    {
        private T _value;
        public T value
        {
            get 
            { 
                return _value; 
            }
            set
            {
                _value = value;
                OnValueChanged.Invoke(value);
            }
        }

        object IConsoleVariable.value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = (T)value;
            }
        }

        public UnityEvent<T> OnValueChanged;

        public ConsoleVariable(T value)
        {
            this._value = value;
            OnValueChanged = new UnityEvent<T>();
        }

    }

    [System.Serializable]
    public abstract class ConsoleCommand
    {
        protected string _command;
        public string command { get { return _command; } }
        public abstract ConsoleOutput ProcessCommand(string[] args);
    }

    public struct ConsoleOutput
    {
        public enum Type
        {
            ERR, // Error
            AST, // Assert
            WRN, // Warning
            INF, // Informative/Unknown
            EXC  // Exception
        }

        public DateTime timeStamp;
        public Type type;
        public string message;

        public ConsoleOutput(Type type, string message)
        {
            this.timeStamp = DateTime.Now;
            this.type = type;
            this.message = message;
        }

        public override string ToString()
        {
            string output = "[" + timeStamp.ToString("u") + "]";
            output += "[" + type.ToString() + "] ";
            output += message;
            return output;
        }
    }

    public class ConsoleLogMetrics
    {
        private Dictionary<ConsoleOutput.Type, int> _metrics = new Dictionary<ConsoleOutput.Type, int>();

        public ConsoleLogMetrics() 
        {
            foreach(var type in Enum.GetValues(typeof(ConsoleOutput.Type)).Cast<ConsoleOutput.Type>())
            {
                _metrics[type] = 0;
            }
        }

        public int GetLogCount()
        {
            return _metrics.Values.Sum();
        }

        public int GetLogCount(ConsoleOutput.Type type)
        {
            return _metrics[type];
        }

        internal int this[ConsoleOutput.Type type]
        {
            get => _metrics[type];
            set => _metrics[type] = value;
        }
    }

    /// <summary>
    /// Represents the runtime of dev console
    /// </summary>
    public class DeveloperConsole
    {
        private Dictionary<string, IConsoleVariable> _cvars = new Dictionary<string, IConsoleVariable>();
        private List<ConsoleCommand> _commands = new List<ConsoleCommand>();
        private int _consoleLogBufferSize = 100;
        private List<ConsoleOutput> _consoleLogs = new List<ConsoleOutput>();
        private ConsoleLogMetrics _metrics = new ConsoleLogMetrics();
        private UnityEvent<ConsoleOutput> _consoleLogged = new UnityEvent<ConsoleOutput>();

#if KRAKEN_ENABLE_LOG_FILE_GEN
        private string _logfilePath = string.Empty;
#endif

        internal int LogBufferSize { get => _consoleLogBufferSize; set => _consoleLogBufferSize = value; }
        internal List<ConsoleOutput> Logs  => _consoleLogs; 
        internal ConsoleLogMetrics Metrics => _metrics;
        internal UnityEvent<ConsoleOutput> OnConsoleLogged => _consoleLogged;

        internal DeveloperConsole()
        {
            Application.logMessageReceived += InterceptDebugLogs;
            Application.quitting += OnApplicationQuit;

#if KRAKEN_ENABLE_LOG_FILE_GEN
            _logfilePath = Application.persistentDataPath + "/Kraken_DevCon_Log_" + DateTime.Now.ToString("'yyyy'_'MM'_'dd'_'HH'_'mm'_'ss'") + ".log";
            File.Create(_logfilePath).Close();
#endif
        }

        private void InterceptDebugLogs(string condition, string stackTrace, LogType type)
        {
            Log((ConsoleOutput.Type)type, condition);
        }

        private async void OnApplicationQuit()
        {
            await Flush();
        }

        /// <summary>
        /// Use this to register new console variables
        /// </summary>
        /// <param name="identifier">Name of cvar</param>
        /// <param name="value">Value of cvar (as a string)</param>
        /// <returns>ConsoleVariable. Can be used to bind functions to OnValueChanged event or change the value itself.</returns>
        internal ConsoleVariable<T> RegisterCVar<T>(string identifier, T value)
        {
            if (_cvars.ContainsKey(identifier))
            {
                Debug.LogError("Attempted to register already existing cvar : " + identifier + ". Existing CVar returned.");
                return (ConsoleVariable<T>)_cvars[identifier];
            }

            var cvarObj = new ConsoleVariable<T>(value);
            _cvars.Add(identifier, cvarObj);

            return cvarObj;
        }

        /// <summary>
        /// Get a ConsoleVariable to change its value or bind to OnValueChanged event.
        /// </summary>
        /// <param name="identifier">Name of cvar</param>
        /// <returns>ConsoleVariable (empty, unregistered cvar if variable doesn't exist</returns>
        internal ConsoleVariable<T> GetCVar<T>(string identifier)
        {
            if( _cvars.ContainsKey(identifier))
                return (ConsoleVariable<T>)_cvars[identifier];
            Debug.LogError("Cvar " + identifier + " not found! Returning empty CVar Object");
            return null;
        }

        internal void ProcessQuery(string query)
        {
            var substrings = query.Split(" ");

            if (substrings.Length == 0)
            {
                Log(ConsoleOutput.Type.WRN, "Empty query submitted!");
                return;
            }

            if (_cvars.ContainsKey(substrings[0]))
            {
                var cvar = _cvars[substrings[0]];
                var old = cvar.value;
                cvar.value = TypeDescriptor.GetConverter(cvar.value.GetType()).ConvertFromString(substrings[1]);
                Log(ConsoleOutput.Type.INF, substrings[0] + " : " + old.ToString() + " -> " + cvar.value.ToString());
                return;
            }

            var cmd = _commands.Find(x => x.command == substrings[0]);
            if (cmd != null)
            {
                var args = substrings.Skip(1).ToArray();
                Log(cmd.ProcessCommand(args));
                return;
            }

            Log(ConsoleOutput.Type.WRN, "No suitable query or command found for \"" + query + "\"!");
        }

        internal void Log(ConsoleOutput.Type type, string message)
        {
            Log(new ConsoleOutput(type, message));
        }

        internal async void Log(ConsoleOutput log) 
        {
            if (_consoleLogs.Count > _consoleLogBufferSize)
            {
                await Flush();
            }

            _consoleLogs.Add(log);
            _metrics[log.type]++;
            OnConsoleLogged.Invoke(_consoleLogs.Last());
        }

        internal bool RegisterCommand(ConsoleCommand command)
        {
            if (_commands.Contains(command)) return false;
            _commands.Add(command);
            return true;
        }

        internal async Task Flush()
        {
#if KRAKEN_ENABLE_LOG_FILE_GEN
            foreach(var log in _consoleLogs)
            {
                await File.AppendAllTextAsync(_logfilePath, log.ToString() + "\n");
            }
#endif
            _consoleLogs.Clear();
        }
    }
}