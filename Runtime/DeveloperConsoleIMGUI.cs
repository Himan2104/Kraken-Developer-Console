using Kraken.DevCon;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly : InternalsVisibleTo("Kraken.DeveloperConsole.Editor")]
public class DeveloperConsoleIMGUI : DeveloperConsoleUI
{
    private DeveloperConsole _console;

    [HideInInspector][SerializeField] internal GUISkin skin;

    string _logs = string.Empty;

    //imgui  vars
    private string cmd = "";
    private Vector2 scrollPos;

    protected override void Initialize()
    {
        //skin = Resources.Load<GUISkin>("Packages/com.kraken.developer-console/Resources/CustomGUISkin.guiskin");
    }

    protected override void OnToggleConsole()
    {
        cmd = string.Empty;
    }

    protected override void AppendLog(ConsoleOutput conop)
    {
        _logs += "<color=#" + HexColorMap[conop.type] + ">" + conop.ToString() + "</color>\n";
    }

    protected override void OnSubmit(string query)
    {
        cmd = "";
    }

    internal override void ClearLogs()
    {
        _logs = string.Empty;
    }

    private void OnGUI()
    {
        if (bIsOpen)
        {
            skin.box.fontSize =
            skin.label.fontSize =
            skin.textArea.fontSize =
            skin.textField.fontSize = (Screen.height / 25) / 2;

            GUI.skin = skin;

            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height / 2));
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height / 2), "");

            scrollPos = GUI.BeginScrollView(new Rect(5, 5, Screen.width - 10, Screen.height / 2 - Screen.height / 20), scrollPos, new Rect(0, 0, 5000, 5000));
            GUI.Box(new Rect(0, 0, 5000, 5000), _logs);
            GUI.EndScrollView();


            cmd = GUI.TextField(new Rect(Screen.height / 25, Screen.height / 2 - Screen.height / 25, Screen.width, Screen.height / 25), cmd);
            GUI.Label(new Rect(0, Screen.height / 2 - Screen.height / 25, Screen.height / 25, Screen.height / 25), ">>");

            GUI.EndGroup();
        }
        if (Event.current.type == EventType.KeyUp)
        {
            if (bIsOpen)
            {
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    SubmitQuery(cmd);
                }
            }

            if (Event.current.keyCode == KeyCode.BackQuote || Event.current.keyCode == KeyCode.Tilde)
            {
                ToggleConsole();
            }
        }
        
    }

    
}
