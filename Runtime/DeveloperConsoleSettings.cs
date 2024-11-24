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
    
        internal const string settingsAssetPath = "Assets/Settings/DeveloperConsoleSettings.asset";
    
        [SerializeField] private ConsoleUIMode _consoleUIMode = ConsoleUIMode.IMGUI;
        [SerializeField] private bool _generateLogFile;
        [SerializeField][Tooltip("Text color for normal informational entries")] private Color _infoColor = Color.white;
        [SerializeField][Tooltip("Text color for warnings")] private Color _warningColor = Color.yellow;
        [SerializeField][Tooltip("Text color for errors")] private Color _errorColor = Color.red;
        [SerializeField][Tooltip("Text color for assertions")] private Color _assertColor = Color.cyan;
        [SerializeField][Tooltip("Text color for exceptions")] private Color _exceptionColor = Color.magenta;
    }
}