using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Kraken.DeveloperConsole.Editor")]

namespace Kraken
{
    public class DeveloperConsoleSettings : ScriptableObject
    {
        public enum ConsoleUIMode
        {
            IMGUI,
            UGUI
        }
    
        internal const string settingsAssetPath = "Assets/Resources/DeveloperConsoleSettings.asset";
    
        [SerializeField] private ConsoleUIMode _consoleUIMode = ConsoleUIMode.IMGUI;
        [SerializeField] private bool _generateLogFile;
        [SerializeField] private bool _shouldPauseGameplay = true;
        [SerializeField] private int _consoleBufferSize = 1024;
        [SerializeField][Tooltip("Text color for normal informational entries")] private Color _infoColor = Color.white;
        [SerializeField][Tooltip("Text color for warnings")] private Color _warningColor = Color.yellow;
        [SerializeField][Tooltip("Text color for errors")] private Color _errorColor = Color.red;
        [SerializeField][Tooltip("Text color for assertions")] private Color _assertColor = Color.cyan;
        [SerializeField][Tooltip("Text color for exceptions")] private Color _exceptionColor = Color.magenta;
        
        internal ConsoleUIMode consoleUIMode => _consoleUIMode;
        internal bool shouldPauseGameplay => _shouldPauseGameplay;
        internal int consoleBufferSize => _consoleBufferSize;
        internal Color infoColor => _infoColor;
        internal Color warningColor => _warningColor;
        internal Color errorColor => _errorColor;
        internal Color assertColor => _assertColor;
        internal Color exceptionColor => _exceptionColor;
    }
}