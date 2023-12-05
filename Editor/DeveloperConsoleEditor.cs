using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Kraken.DevCon
{
    public class DeveloperConsoleEditor : Editor
    {
        private const string generate_log_file_epkey = "kraken_enable_log_file";

        [MenuItem("Kraken/Developer Console/Create UGUI Console Object", priority = 1)]
        static void CreateDeveloperConsoleUGUI()
        {
            if(EditorSceneManager.GetSceneByBuildIndex(0) != EditorSceneManager.GetActiveScene()) 
            {
                EditorUtility.DisplayDialog("Incorrect Scene", 
                    "The current scene is not a initializer scene or the first scene" +
                    "in your game. Developer Console is a DDOL object and must be initialized " +
                    "in a scene which loads first i.e., whose build index is 0."," OK");
                return;
            }

            if (FindObjectOfType<DeveloperConsoleUI>())
            {
                EditorUtility.DisplayDialog("Already exists", "A Developer Console object already exists in this scene." +
                    "Please remove it before trying again.", "OK");
                return;
            }

            //main canvas game object
            GameObject console = new GameObject("DeveloperConsole");

            console.transform.parent = null;
            console.transform.localPosition = Vector3.zero;
            console.transform.localRotation = Quaternion.identity;
            console.transform.localScale = Vector3.one;

            var canvas = console.AddComponent<Canvas>();
            canvas.sortingOrder = 0;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;

            var canvasScaler = console.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.referencePixelsPerUnit = 100f;

            console.AddComponent<GraphicRaycaster>();

            //scrollable output panel
            GameObject outputPanel = new GameObject("OutputPanel");
            outputPanel.AddComponent<RectTransform>();

            var outputPanel_transform = outputPanel.GetComponent<RectTransform>();
            outputPanel_transform.SetParent(console.transform, false);
            outputPanel_transform.anchorMin = Vector2.up;
            outputPanel_transform.anchorMax = Vector2.one;
            outputPanel_transform.sizeDelta = new Vector2(0, 300);
            outputPanel_transform.anchoredPosition = new Vector2(0, 250);

            outputPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);

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
            consoleText_textComp.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Packages/com.kraken.developer-console/CONSOLA SDF.asset");
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

            scrollBar.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
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

            scrollHandle.AddComponent<Image>().color = Color.white;

            scrollBarComp.handleRect = scrollHandle_transform;
            scrollBarComp.direction = Scrollbar.Direction.BottomToTop;
            scrollBarComp.value = 0;
            scrollBarComp.numberOfSteps = 0;

            outputPanel_scrollRect.verticalScrollbar = scrollBarComp;

            //Input Field
            GameObject inputBox = new GameObject("InputBox");
            var inputBox_transform = inputBox.AddComponent<RectTransform>();
            inputBox_transform.SetParent(console.transform, false);
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
            image.color = new Color(0,0,0,0.5f);

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
            text.enableWordWrapping = false;
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
            placeholder.enableWordWrapping = false;
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


            var devconui = console.AddComponent<DeveloperConsoleUGUI>();
            devconui._output = consoleText_textComp;
            devconui._input = inputField;
            devconui._outputPanel = outputPanel;
            devconui._inputPanel = inputBox;

            Undo.RegisterCreatedObjectUndo(console, "Create " + console.name);
        }

        [MenuItem("Kraken/Developer Console/Create IMGUI Console Object", priority = 2)]
        static void CreateDeveloperConsoleIMGUI()
        {
            if (EditorSceneManager.GetSceneByBuildIndex(0) != EditorSceneManager.GetActiveScene())
            {
                EditorUtility.DisplayDialog("Incorrect Scene",
                    "The current scene is not a initializer scene or the first scene" +
                    "in your game. Developer Console is a DDOL object and must be initialized " +
                    "in a scene which loads first i.e., whose build index is 0.", " OK");
                return;
            }

            if (FindObjectOfType<DeveloperConsoleUI>())
            {
                EditorUtility.DisplayDialog("Already exists", "A Developer Console object already exists in this scene." +
                    "Please remove it before trying again.", "OK");
                return;
            }
            var ui = new GameObject("DeveloperConsole").AddComponent<DeveloperConsoleIMGUI>();
            ui.skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Packages/com.kraken.developer-console/Resources/CustomGUISkin.guiskin");
            EditorSceneManager.SaveOpenScenes();
            Undo.RegisterCreatedObjectUndo(ui, "Create " + ui.name);
        }

        private static bool bShouldGenerateLogFile
        {
            get => EditorPrefs.GetBool(generate_log_file_epkey);
            set => EditorPrefs.SetBool(generate_log_file_epkey, value);
        }

        [MenuItem("Kraken/Developer Console/Generate Log File", priority = 20)]
        static void GenerateLogFile()
        {
            bShouldGenerateLogFile = !bShouldGenerateLogFile;

            if (bShouldGenerateLogFile)
            {
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, "KRAKEN_ENABLE_LOG_FILE_GEN");
            }
            else
            {
                string[] defines;
                PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, out defines);
                var new_defines = defines.ToList().Where(x => !string.Equals(x, "KRAKEN_ENABLE_LOG_FILE_GEN")).ToArray();
                if (new_defines.Length > 0)
                {
                    PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, new_defines);
                }
            }
        }
        
        [MenuItem("Kraken/Developer Console/Generate Log File", true)]
        static bool GenerateLogFileValidate()
        {
            Menu.SetChecked("Kraken/Developer Console/Generate Log File", bShouldGenerateLogFile);
            return true;
        }
    }
}