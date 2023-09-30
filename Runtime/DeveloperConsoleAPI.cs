using UnityEngine;

namespace Kraken.DevCon
{
    public static class DeveloperConsoleAPI
    {
        private static DeveloperConsole _con = null;
        private static DeveloperConsoleUI _ui = null;

        private static float _timeScale = 1.0f;

        public static bool bIsInitialized
        {
            get { return _con != null; }
        }

        public static bool bGenerateLogFile = false;

        /// <summary>
        /// Initializes the developer console. Called during awkae() of DeveloperConsoleUI
        /// </summary>
        internal static DeveloperConsole Initialize(DeveloperConsoleUI dev_con_ui)
        {
            if(_ui != null && _con != null)
            {
                Debug.LogWarning("Attempted to re-initialize Developer Console!");
                return _con;
            }

            _con = new DeveloperConsole();
            _ui = dev_con_ui;
            return _con;
        }

        /// <summary>
        /// Toggle console (on/off)
        /// </summary>
        public static void ToggleConsole()
        {
            if (_con == null || _ui == null)
            {
                Debug.LogError("Console hasn't been created yet!");
                return;
            }

            if(!_ui._is_open)
            {
                _timeScale = Time.timeScale;
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = _timeScale;
            }
            
            _ui.ToggleConsole();
        }

        /// <summary>
        /// Sets the buffer size for console logs (default is 100). Also wipes console when called.
        /// </summary>
        /// <param name="buffer_size">number of logs to store</param>
        public static void SetConsoleBufferSize(int buffer_size)
        {
            if (buffer_size < 0)
            {
                Debug.LogError("Invalid Buffer Size!");
                return;
            }
            if(_con == null)
            {
                Debug.LogError("Console has not been created!");
                return;
            }
            _con._consoleLogs.Clear();
            _con._consoleLogBufferSize = buffer_size;
            _ui.RefreshLogs();
        }

        /// <summary>
        /// Log something to the console.
        /// </summary>
        /// <param name="type">Type of log (ConsoleOutput.Type)</param>
        /// <param name="message">Message as a string</param>
        public static void Log(ConsoleOutput.Type type, string message)
        {
            _ui.AppendLog(_con.Log(type, message));
        }

        /// <summary>
        /// Used to register a new command.
        /// </summary>
        /// <param name="command">Any child of ConsoleCommand (abstract)</param>
        /// <returns>true : success | false : command already exists</returns>
        public static bool RegisterCommand(ConsoleCommand command)
        {
            return _con.RegisterCommand(command);
        }

        /// <summary>
        /// Register a new console variable.
        /// </summary>
        /// <typeparam name="T">Type of variable</typeparam>
        /// <param name="identifier">Variable name</param>
        /// <param name="value">Init value</param>
        /// <returns>Console Variable Object</returns>
        public static ConsoleVariable<T> RegisterCVar<T>(string identifier, T value)
        {
            return _con.RegisterCVar<T>(identifier, value);
        }

        /// <summary>
        /// Retrieves a previously registered console variable.
        /// </summary>
        /// <typeparam name="T">Variable type</typeparam>
        /// <param name="identifier">Name of the variable</param>
        /// <returns>Console Variable Objectt. Returns null if variable doesn't exist</returns>
        public static ConsoleVariable<T> GetCVar<T>(string identifier)
        {
            return _con.GetCVar<T>(identifier);
        }

        /// <summary>
        /// Clear all logs. Writes onto the logfile if bGenerateLogFile is set to true.
        /// </summary>
        public static void Flush()
        {
            _con.UpdateLogFile();
            _con._consoleLogs.Clear();
            _ui._output.text = "KRAKEN DEVELOPER CONSOLE\n.\n.\n.\n.\n.\n.\n.\n\nCreated By - Himanshu Parchand (himan2104@gmail.com)\n.\n.\n.\n.\n.\n.\n\nPlease read the documentation before using and leave a star on the repository if this helped you!\n.\n.\n.\n.\n.\n\n";
        }
    }
}
