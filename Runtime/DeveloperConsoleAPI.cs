using UnityEngine;

namespace Kraken.DevCon
{
    public static class DeveloperConsoleAPI
    {
        private static DeveloperConsole _con = null;
        private static DeveloperConsoleUI _ui = null;

        public static bool bIsOpen => _ui ? _ui.bIsOpen : false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Initialize()
        {
            var settings = Resources.Load<DeveloperConsoleSettings>("DeveloperConsoleSettings");
            
            _con = new DeveloperConsole();
            if (settings.consoleUIMode == DeveloperConsoleSettings.ConsoleUIMode.IMGUI)
            {
                _ui = new GameObject("Developer Console").AddComponent<DeveloperConsoleIMGUI>();
            }
            else
            {
                _ui = new GameObject("DeveloperConsole").AddComponent<DeveloperConsoleUGUI>();
            }
            _ui.Initialize(_con, settings);
        }

        /// <summary>
        /// Toggle console (on/off).
        /// </summary>
        public static void ToggleConsole()
        {
            if (_con == null || _ui == null)
            {
                Debug.LogError("Console hasn't been created yet!");
                return;
            }
            
            _ui.ToggleConsole();
        }

        /// <summary>
        /// Log something to the console.
        /// </summary>
        /// <param name="type">Type of log (ConsoleOutput.Type)</param>
        /// <param name="message">Message as a string</param>
        public static void Log(ConsoleOutput.Type type, string message)
        {
            _con.Log(type, message);
        }

        /// <summary>
        /// Used to register a new command.
        /// </summary>
        /// <typeparam name="T">Sub class of IConsoleCommand</typeparam>
        /// <param name="command">Name of the command used to call it</param>
        /// <returns>true : success | false : command already exists</returns>
        public static bool RegisterCommand<T>(string command) where T : IConsoleCommand, new()
        {
            return _con.RegisterCommand<T>(command);
        }

        /// <summary>
        /// Used to deregister a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool DeregisterCommand<T>(string command) where T : IConsoleCommand, new()
        {
            return _con.DeregisterCommand<T>(command);
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
        public static async void Flush()
        {
            await _con.Flush();
            //uncomment to assert dominance XD
            //_ui._output.text = "KRAKEN DEVELOPER CONSOLE\n.\n.\n.\n.\n.\n.\n.\n\nCreated By - Himanshu Parchand (himan2104@gmail.com)\n.\n.\n.\n.\n.\n.\n\nPlease read the documentation before using and leave a star on the repository if this helped you!\n.\n.\n.\n.\n.\n\n";
        }
    }
}
