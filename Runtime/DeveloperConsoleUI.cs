using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kraken.DevCon
{
    public abstract class DeveloperConsoleUI : MonoBehaviour
    {
        private DeveloperConsole _console;
        private Dictionary<ConsoleOutput.Type, string> _hexColorMap = new Dictionary<ConsoleOutput.Type, string>();
        private bool _isOpen = false;
        private float _timeScale = 1.0f;
        private CursorLockMode _cursorLockMode = CursorLockMode.None;
        private bool _pauseGameplay = true;
        
        protected DeveloperConsole Console => _console;
        protected Dictionary<ConsoleOutput.Type, string> hexColorMap => _hexColorMap;
        internal bool bIsOpen => _isOpen; 

        protected abstract void OnInitialize();
        protected abstract void OnToggleConsole();
        protected abstract void AppendLog(ConsoleOutput conop);
        protected abstract void OnSubmit(string query);
        internal abstract void ClearLogs();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        internal void Initialize(DeveloperConsole console, DeveloperConsoleSettings settings)
        {
            SetupColors(settings);
            _console = console;
            _console.OnConsoleLogged.AddListener(AppendLog);
            _console.LogBufferSize = settings.consoleBufferSize;
            _pauseGameplay = settings.shouldPauseGameplay;
            OnInitialize();
        }

        private void SetupColors(DeveloperConsoleSettings settings)
        {
            _hexColorMap[ConsoleOutput.Type.INF] = ColorUtility.ToHtmlStringRGBA(settings.infoColor);
            _hexColorMap[ConsoleOutput.Type.WRN] = ColorUtility.ToHtmlStringRGBA(settings.warningColor);
            _hexColorMap[ConsoleOutput.Type.ERR] = ColorUtility.ToHtmlStringRGBA(settings.errorColor);
            _hexColorMap[ConsoleOutput.Type.AST] = ColorUtility.ToHtmlStringRGBA(settings.assertColor);
            _hexColorMap[ConsoleOutput.Type.EXC] = ColorUtility.ToHtmlStringRGBA(settings.exceptionColor);
        }

        internal void ToggleConsole()
        {
            _isOpen = !_isOpen;

            if (_pauseGameplay)
            {
                if (bIsOpen)
                {
                    _timeScale = Time.timeScale;
                    Time.timeScale = 0.0f;
                    _cursorLockMode = Cursor.lockState;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Time.timeScale = _timeScale;
                    Cursor.lockState = _cursorLockMode;
                }
            }

            OnToggleConsole();
        }

        /// <summary>
        /// Internal implentation to submit the input query
        /// </summary>
        /// <param name="query"></param>
        internal void SubmitQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return;
            _console.ProcessQuery(query);
            OnSubmit(query);
        }

        /// <summary>
        /// Refreshes the logs in the output
        /// </summary>
        internal void RefreshLogs()
        {
            ClearLogs();
            foreach(var log in _console.Logs)
            {
                AppendLog(log);
            }
        }

        private void OnApplicationQuit()
        {
            if (_isOpen)
            {
                ToggleConsole();
                Debug.Log("RESET_CON");
            }
        }
    }
}