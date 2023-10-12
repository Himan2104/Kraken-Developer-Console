using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kraken.DevCon
{
    public abstract class DeveloperConsoleUI : MonoBehaviour
    {
        [SerializeField][Tooltip("Text color for normal informational entries")] private Color InfoColor = Color.white;
        [SerializeField][Tooltip("Text color for warnings")] private Color WarningColor = Color.yellow;
        [SerializeField][Tooltip("Text color for errors")] private Color ErrorColor = Color.red;
        [SerializeField][Tooltip("Text color for assertions")] private Color AssertColor = Color.cyan;
        [SerializeField][Tooltip("Text color for exceptions")] private Color ExceptionColor = Color.magenta;

        private DeveloperConsole _console;
        private Dictionary<ConsoleOutput.Type, string> _hexColorMap = new Dictionary<ConsoleOutput.Type, string>();
        private bool _isOpen = false;
        
        protected DeveloperConsole Console => _console;
        protected Dictionary<ConsoleOutput.Type, string> HexColorMap => _hexColorMap;
        internal bool bIsOpen => _isOpen; 

        protected abstract void Initialize();
        protected abstract void OnToggleConsole();
        protected abstract void AppendLog(ConsoleOutput conop);
        protected abstract void OnSubmit(string query);
        internal abstract void ClearLogs();

        private void Awake()
        {
            _console = DeveloperConsoleAPI.Initialize(this);
            DontDestroyOnLoad(this);
            SetupColors();
            _console.OnConsoleLogged.AddListener(AppendLog);
            Initialize();
        }

        private void SetupColors()
        {
            _hexColorMap[ConsoleOutput.Type.INF] = ColorUtility.ToHtmlStringRGBA(InfoColor);
            _hexColorMap[ConsoleOutput.Type.WRN] = ColorUtility.ToHtmlStringRGBA(WarningColor);
            _hexColorMap[ConsoleOutput.Type.ERR] = ColorUtility.ToHtmlStringRGBA(ErrorColor);
            _hexColorMap[ConsoleOutput.Type.AST] = ColorUtility.ToHtmlStringRGBA(AssertColor);
            _hexColorMap[ConsoleOutput.Type.EXC] = ColorUtility.ToHtmlStringRGBA(ExceptionColor);
        }

        internal void ToggleConsole()
        {
            _isOpen = !_isOpen;
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