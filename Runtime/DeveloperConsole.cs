using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            NUL, // Null/UnknownType
            ERR, // Error
            INF, // Informative
            WRN  // Warning
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

    /// <summary>
    /// Represents the runtime of dev console
    /// </summary>
    public class DeveloperConsole
    {
        private Dictionary<string, IConsoleVariable> _cvars = new Dictionary<string, IConsoleVariable>();

        private List<ConsoleCommand> _commands = new List<ConsoleCommand>();

        internal int _consoleLogBufferSize = 100;

        internal List<ConsoleOutput> _consoleLogs = new List<ConsoleOutput>();

        internal int _clStatWarnings = 0;
        internal int _clStatErrors = 0;

        internal DeveloperConsole()
        {
            Application.quitting += OnApplicationQuit;
        }

        private void OnApplicationQuit()
        {
            UpdateLogFile();
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

        internal ConsoleOutput ProcessQuery(string query)
        {
            var substrings = query.Split(" ");

            if (substrings.Length == 0) 
                return new ConsoleOutput(ConsoleOutput.Type.WRN, "Empty query submitted!");

            if (_cvars.ContainsKey(substrings[0]))
            {
                var cvar = _cvars[substrings[0]];
                var old = cvar.value;
                cvar.value = TypeDescriptor.GetConverter(cvar.value.GetType()).ConvertFromString(substrings[1]);
                return new ConsoleOutput(ConsoleOutput.Type.INF, substrings[0] + " : " + old.ToString() + " -> " + cvar.value.ToString());
            }

            var cmd = _commands.Find(x => x.command == substrings[0]);
            if (cmd != null)
            {
                var args = substrings.Skip(1).ToArray();
                return cmd.ProcessCommand(args);
            }

            return new ConsoleOutput(ConsoleOutput.Type.WRN, "No suitable query or command found for \"" + query + "\"!");
        }

        internal ConsoleOutput Log(ConsoleOutput.Type type, string message)
        {
            if(_consoleLogs.Count > _consoleLogBufferSize)
            {
                File.AppendAllTextAsync(Application.persistentDataPath + "Kraken_DevCon_Log_" + DateTime.Now.ToString("u"), _consoleLogs[0].ToString() + "\n");
                _consoleLogs.RemoveAt(0);
            }

            _consoleLogs.Add(new ConsoleOutput(type, message));
            
            return _consoleLogs.Last();
        }

        internal bool RegisterCommand(ConsoleCommand command)
        {
            if (_commands.Contains(command)) return false;
            _commands.Add(command);
            return true;
        }

        internal void UpdateLogFile()
        {
            foreach(var log in _consoleLogs)
            {
                File.AppendAllTextAsync(Application.persistentDataPath + "Kraken_DevCon_Log_" + DateTime.Now.ToString("u"), log.ToString() + "\n");
            }
        }
    }
}