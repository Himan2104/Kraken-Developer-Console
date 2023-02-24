using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[assembly : InternalsVisibleTo("kraken.developer-console.Editor")]

namespace Kraken.DevCon
{    
    public class DeveloperConsoleUI : MonoBehaviour
    {
        private DeveloperConsole _console;

        //These references need not be shown in inspector.
        //Serializing them is neccessary or they won't hold their refs in runtime.
        [HideInInspector][SerializeField] internal TextMeshProUGUI _output;
        [HideInInspector][SerializeField] internal TMP_InputField _input;
        [HideInInspector][SerializeField] internal GameObject _output_panel;
        [HideInInspector][SerializeField] internal GameObject _input_panel;

        //No need to hide these since these colors need to be modifiable by the devs.
        [SerializeField] [Tooltip("Text color for normal informational entries")] private Color InfoColor = Color.cyan;
        [SerializeField] [Tooltip("Text color for warnings")] private Color WarningColor = Color.yellow;
        [SerializeField] [Tooltip("Text color errors")] private Color ErrorColor = Color.red;
        [SerializeField] [Tooltip("Text color for unknown type of entries")] private Color UnknownColor = Color.white;

        //Hex strings of the above declared Colors.
        private string _info_color;
        private string _warning_color;
        private string _error_color;
        private string _unknown_color;

        internal bool _is_open = false;

        private void Awake()
        {
            _console = DeveloperConsoleAPI.Initialize(this);
            DontDestroyOnLoad(this);
            _input.onSubmit.AddListener(OnSubmit);
            SetupColors();
        }

        /// <summary>
        /// Animate the toggling of console. Also sets bIsOpen.
        /// </summary>
        internal async void ToggleConsole()
        {
            var i_ancpos = _input_panel.GetComponent<RectTransform>().anchoredPosition;
            var o_ancpos = _output_panel.GetComponent<RectTransform>().anchoredPosition;
            if (!_is_open)
            {
                _is_open = true;
                _input.Select();
                while (i_ancpos.y > -320 && o_ancpos.y > -150)
                {
                    _output_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(o_ancpos, new Vector2(0, -150), 10.0f * Time.unscaledDeltaTime);
                    _input_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(i_ancpos, new Vector2(0, -320), 10.0f * Time.unscaledDeltaTime);
                    i_ancpos = _input_panel.GetComponent<RectTransform>().anchoredPosition;
                    o_ancpos = _output_panel.GetComponent<RectTransform>().anchoredPosition;
                    await Task.Yield();
                }
            }
            else
            {
                _is_open = false;
                _input.ReleaseSelection();
                while (i_ancpos.y < 80 && o_ancpos.y < 250)
                {
                    _output_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(o_ancpos, new Vector2(0, 250), 10.0f * Time.unscaledDeltaTime);
                    _input_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(i_ancpos, new Vector2(0, 80), 10.0f * Time.unscaledDeltaTime);
                    i_ancpos = _input_panel.GetComponent<RectTransform>().anchoredPosition;
                    o_ancpos = _output_panel.GetComponent<RectTransform>().anchoredPosition;
                    await Task.Yield();
                }
            }
        }

        /// <summary>
        /// Appends a new line to the output console.
        /// </summary>
        /// <param name="conop"></param>
        internal void AppendLog(ConsoleOutput conop)
        {
            string color = "";
            switch(conop.type)
            {
                case ConsoleOutput.Type.NUL:
                    color = _unknown_color;
                    break;
                case ConsoleOutput.Type.INF:
                    color = _info_color;
                    break;
                case ConsoleOutput.Type.WRN:
                    color = _warning_color;
                    break;
                case ConsoleOutput.Type.ERR:
                    color = _error_color;
                    break;
            }

            _output.text += "<color=#" + color + ">" + conop.ToString() + "</color>\n";
            DelayedScrollSnap();
        }

        /// <summary>
        /// Wipes console and reloads all logs.
        /// </summary>
        internal void RefreshLogs()
        {
            _output.text = "";
            foreach(var log in _console._console_logs)
            {
                AppendLog(log);
            }
        }

        /// <summary>
        /// Called when command field triggers onEndEdit.
        /// </summary>
        /// <param name="query"></param>
        internal void OnSubmit(string query)
        {
            if (query.Length == 0) return;
            _input.text = "";
            AppendLog(_console.ProcessQuery(query));
        }

        /// <summary>
        /// Converts UnityEngine.Color to HexStrings. 
        /// </summary>
        private void SetupColors()
        {
            _info_color = ColorUtility.ToHtmlStringRGBA(InfoColor);
            _warning_color = ColorUtility.ToHtmlStringRGBA(WarningColor);
            _error_color = ColorUtility.ToHtmlStringRGBA(ErrorColor);
            _unknown_color = ColorUtility.ToHtmlStringRGBA(UnknownColor);
        }

        private void OnApplicationQuit()
        {
            if (_is_open) ToggleConsole();
        }

        /// <summary>
        /// Scrolls to the bottom. Delayed cause it takes a certain amount of time for Content Size Fitter to adjust
        /// </summary>
        /// <param name="delay"></param>
        private async void DelayedScrollSnap(int delay = 10)
        {
            await Task.Delay(delay);
            GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0;
        }
    }
}