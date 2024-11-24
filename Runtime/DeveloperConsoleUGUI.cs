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

        protected override void OnInitialize()
        {

            gameObject.transform.parent = null;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.sortingOrder = 0;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.referencePixelsPerUnit = 100f;

            gameObject.AddComponent<GraphicRaycaster>();

            //scrollable output panel
            GameObject outputPanel = new GameObject("OutputPanel");
            outputPanel.AddComponent<RectTransform>();

            var outputPanel_transform = outputPanel.GetComponent<RectTransform>();
            outputPanel_transform.SetParent(gameObject.transform, false);
            outputPanel_transform.anchorMin = Vector2.up;
            outputPanel_transform.anchorMax = Vector2.one;
            outputPanel_transform.sizeDelta = new Vector2(0, 300);
            outputPanel_transform.anchoredPosition = new Vector2(0, 250);

            outputPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);

            var outputPanel_scrollRect = outputPanel.AddComponent<ScrollRect>();
            outputPanel_scrollRect.horizontal = false;
            outputPanel_scrollRect.vertical = true;
            outputPanel_scrollRect.movementType = ScrollRect.MovementType.Clamped;
            outputPanel_scrollRect.inertia = false;
            outputPanel_scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            //output panel viewport
            GameObject viewport = new GameObject("Viewport");
            var viewport_transform = viewport.AddComponent<RectTransform>();
            viewport_transform.SetParent(outputPanel_transform, false);
            viewport_transform.anchorMin = Vector2.zero;
            viewport_transform.anchorMax = Vector2.one;
            viewport_transform.pivot = Vector2.up;
            viewport_transform.sizeDelta = Vector2.zero;

            viewport.AddComponent<Image>();
            viewport.AddComponent<Mask>().showMaskGraphic = false;

            //output panel content
            GameObject consoleText = new GameObject("Output");

            var consoleText_textComp = consoleText.AddComponent<TextMeshProUGUI>();
            consoleText_textComp.font = Resources.Load<TMP_FontAsset>("CONSOLA SDF.asset");
            consoleText_textComp.text = "KRAKEN DEVELOPER CONSOLE\n.\n.\n.\n.\n.\n.\n.\n\nCreated By - Himanshu Parchand (himan2104@gmail.com)\n.\n.\n.\n.\n.\n.\n\nPlease read the documentation before using and leave a star on the repository if this helped you!\n.\n.\n.\n.\n.\n\n";
            consoleText_textComp.fontSize = 10.0f;
            consoleText_textComp.margin = new Vector4(10,0,10,0);

            var consoleText_transform = consoleText.GetComponent<RectTransform>();
            consoleText_transform.SetParent(viewport_transform, false);
            consoleText_transform.anchorMin = Vector2.up;
            consoleText_transform.anchorMax = Vector2.one;
            consoleText_transform.sizeDelta = new Vector2(0, 300);
            consoleText_transform.anchoredPosition = new Vector2(0, -150);

            consoleText.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            outputPanel_scrollRect.content = consoleText_transform;

            //scrollbar
            GameObject scrollBar = new GameObject("ScrollBar");

            var scrollbar_transform = scrollBar.AddComponent<RectTransform>();
            scrollbar_transform.SetParent(outputPanel_transform, false);
            scrollbar_transform.anchorMin = Vector2.right;
            scrollbar_transform.anchorMax = Vector2.one;
            scrollbar_transform.pivot = Vector2.one;
            scrollbar_transform.sizeDelta = new Vector2(20, 0);

            scrollBar.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);
            var scrollBarComp = scrollBar.AddComponent<Scrollbar>();

            GameObject slidingArea = new GameObject("SlidingArea");
            var slidingArea_transform = slidingArea.AddComponent<RectTransform>();
            slidingArea_transform.SetParent(scrollbar_transform, false);
            slidingArea_transform.anchorMin = Vector2.zero;
            slidingArea_transform.anchorMax = Vector2.one;
            slidingArea_transform.sizeDelta = Vector2.zero;

            GameObject scrollHandle = new GameObject("Handle");
            var scrollHandle_transform = scrollHandle.AddComponent<RectTransform>();
            scrollHandle_transform.SetParent(slidingArea_transform, false);
            scrollHandle_transform.anchorMax = Vector2.one;
            scrollHandle_transform.sizeDelta = Vector2.zero;

            scrollHandle.AddComponent<Image>().color = Color.gray;

            scrollBarComp.handleRect = scrollHandle_transform;
            scrollBarComp.direction = Scrollbar.Direction.BottomToTop;
            scrollBarComp.value = 0;
            scrollBarComp.numberOfSteps = 0;

            outputPanel_scrollRect.verticalScrollbar = scrollBarComp;

            //Input Field
            GameObject inputBox = new GameObject("InputBox");
            var inputBox_transform = inputBox.AddComponent<RectTransform>();
            inputBox_transform.SetParent(gameObject.transform, false);
            inputBox_transform.anchorMin = Vector2.up;
            inputBox_transform.anchorMax = Vector2.one;
            inputBox_transform.anchoredPosition = new Vector2(0, 80);
            inputBox_transform.sizeDelta = new Vector2(0, 30);


            GameObject textArea = new GameObject("Text Area");
            textArea.AddComponent<RectTransform>().SetParent(inputBox.transform, false);

            GameObject childPlaceholder = new GameObject("Placeholder");
            childPlaceholder.AddComponent<RectTransform>().SetParent(textArea.transform, false);

            GameObject childText = new GameObject("Text");
            childText.AddComponent<RectTransform>().SetParent(textArea.transform, false);

            Image image = inputBox.AddComponent<Image>();
            image.type = Image.Type.Sliced;
            image.color = new Color(0,0,0,0.7f);

            TMP_InputField inputField = inputBox.AddComponent<TMP_InputField>();

            // Use UI.Mask for Unity 5.0 - 5.1 and 2D RectMask for Unity 5.2 and up
            RectMask2D rectMask = textArea.AddComponent<RectMask2D>();
#if UNITY_2019_4_OR_NEWER && !UNITY_2019_4_1 && !UNITY_2019_4_2 && !UNITY_2019_4_3 && !UNITY_2019_4_4 && !UNITY_2019_4_5 && !UNITY_2019_4_6 && !UNITY_2019_4_7 && !UNITY_2019_4_8 && !UNITY_2019_4_9 && !UNITY_2019_4_10 && !UNITY_2019_4_11
            rectMask.padding = new Vector4(-8, -5, -8, -5);
#endif
            
            RectTransform textAreaRectTransform = textArea.GetComponent<RectTransform>();
            textAreaRectTransform.anchorMin = Vector2.zero;
            textAreaRectTransform.anchorMax = Vector2.one;
            textAreaRectTransform.sizeDelta = Vector2.zero;
            textAreaRectTransform.offsetMin = new Vector2(10, 6);
            textAreaRectTransform.offsetMax = new Vector2(-10, -7);


            TextMeshProUGUI text = childText.AddComponent<TextMeshProUGUI>();
            text.text = "";
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.extraPadding = true;
            text.richText = true;
            text.color = Color.white;
            text.fontSize = 15;
            text.font = consoleText_textComp.font;

            TextMeshProUGUI placeholder = childPlaceholder.AddComponent<TextMeshProUGUI>();
            placeholder.text = "Enter command";
            placeholder.fontSize = 15;
            placeholder.fontStyle = FontStyles.Italic;
            placeholder.color = new Color(1f, 1f, 1f, 0.5f);
            placeholder.textWrappingMode = TextWrappingModes.NoWrap;
            placeholder.extraPadding = true;
            placeholder.font = consoleText_textComp.font;

            // Add Layout component to placeholder.
            placeholder.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = Vector2.zero;
            placeholderRectTransform.offsetMax = Vector2.zero;

            inputField.textViewport = textAreaRectTransform;
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.fontAsset = text.font;

            _output = consoleText_textComp;
            _input = inputField;
            _outputPanel = outputPanel;
            _inputPanel = inputBox;
            
            _input.onSubmit.AddListener(SubmitQuery);
        }

        internal override void ClearLogs()
        {
            _output.text = string.Empty;
        }

        protected override async void OnToggleConsole() 
        {
            _input.text = string.Empty;
            var i_ancpos = _inputPanel.GetComponent<RectTransform>().anchoredPosition;
            var o_ancpos = _outputPanel.GetComponent<RectTransform>().anchoredPosition;
            if (bIsOpen)
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
            _output.text += "<color=#" + hexColorMap[conop.type] + ">" + conop.ToString() + "</color>\n";
            DelayedScrollSnap();
        }

        protected override void OnSubmit(string query)
        {
            _input.text = "";
            _input.Select();
        }

        private async void DelayedScrollSnap(int delay = 10)
        {
            await Task.Delay(delay);
            if(this != null) GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0;
        }

    }
}