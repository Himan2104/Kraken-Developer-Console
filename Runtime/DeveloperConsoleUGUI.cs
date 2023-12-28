using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("Kraken.DeveloperConsole.Editor")]

namespace Kraken.DevCon
{
    public class DeveloperConsoleUGUI : DeveloperConsoleUI
    {
        //These references need not be shown in inspector.
        //Serializing them is neccessary or they won't hold their refs in runtime.
        [HideInInspector][SerializeField] internal TextMeshProUGUI _output;
        [HideInInspector][SerializeField] internal TMP_InputField _input;
        [HideInInspector][SerializeField] internal GameObject _outputPanel;
        [HideInInspector][SerializeField] internal GameObject _inputPanel;

        protected override void Initialize()
        {
            _input.onSubmit.AddListener(SubmitQuery);
        }

        internal override void ClearLogs()
        {
            _output.text = string.Empty;
        }

        protected override async void OnToggleConsole() 
        {
            var i_ancpos = _inputPanel.GetComponent<RectTransform>().anchoredPosition;
            var o_ancpos = _outputPanel.GetComponent<RectTransform>().anchoredPosition;
            if (!bIsOpen)
            {
                _input.Select();
                while (i_ancpos.y > -320 && o_ancpos.y > -150)
                {
                    _outputPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(o_ancpos, new Vector2(0, -150), 10.0f * Time.unscaledDeltaTime);
                    _inputPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(i_ancpos, new Vector2(0, -320), 10.0f * Time.unscaledDeltaTime);
                    i_ancpos = _inputPanel.GetComponent<RectTransform>().anchoredPosition;
                    o_ancpos = _outputPanel.GetComponent<RectTransform>().anchoredPosition;
                    await Task.Yield();
                }
            }
            else
            {
                _input.ReleaseSelection();
                while (i_ancpos.y < 80 && o_ancpos.y < 250)
                {
                    _outputPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(o_ancpos, new Vector2(0, 250), 10.0f * Time.unscaledDeltaTime);
                    _inputPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(i_ancpos, new Vector2(0, 80), 10.0f * Time.unscaledDeltaTime);
                    i_ancpos = _inputPanel.GetComponent<RectTransform>().anchoredPosition;
                    o_ancpos = _outputPanel.GetComponent<RectTransform>().anchoredPosition;
                    await Task.Yield();
                }
            }
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.BackQuote || Event.current.keyCode == KeyCode.Tilde)
                {
                    ToggleConsole();
                }
            }
        }

        protected override void AppendLog(ConsoleOutput conop)
        {
            _output.text += "<color=#" + HexColorMap[conop.type] + ">" + conop.ToString() + "</color>\n";
            DelayedScrollSnap();
        }

        protected override void OnSubmit(string query)
        {
            _input.text = "";
        }

        private async void DelayedScrollSnap(int delay = 10)
        {
            await Task.Delay(delay);
            if(this != null) GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0;
        }

    }
}