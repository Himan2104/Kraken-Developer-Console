using Kraken.DevCon;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSystem : MonoBehaviour
{
    DemoLib demoLib;

    ConsoleVariable<float> ballSpeed;
    ConsoleVariable<string> ballColor;

    void Start()
    {
        demoLib = new DemoLib(); // all commands and vars are initialized here

        //retrieving the ball color and speed cvars
        ballSpeed = DeveloperConsoleAPI.GetCVar<float>("ballSpeed");
        ballColor = DeveloperConsoleAPI.GetCVar<string>("ballColor");

        //adding a listener to on value changed to modify all balls in the scene
        ballSpeed.OnValueChanged.AddListener(UpdateAllBallSpeeds);
        ballColor.OnValueChanged.AddListener(UpdateAllBallColors);
    }

    private void Update()
    {
        //One of the ways to toggle console
        if (Keyboard.current.backquoteKey.wasReleasedThisFrame)
        {
            DeveloperConsoleAPI.ToggleConsole();
        }
    }

    public void UpdateAllBallSpeeds(float speed)
    {
        var balls = FindObjectsOfType<Ball>();

        foreach (var ball in balls)
        {
            ball.speed = speed;
        }
    }

    public void UpdateAllBallColors(string color)
    {
        var balls = FindObjectsOfType<Ball>();

        Color col;
        if(!ColorUtility.TryParseHtmlString(color, out col)) //if parse fails fallback to white
        {
            DeveloperConsoleAPI.Log(ConsoleOutput.Type.ERR, "Failed to modify Color!");
            col = Color.white;
        }

        foreach (var ball in balls)
        {
            ball.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", col * 2.0f); //multiplied by 2 cause emissive intensity
        }
    }
}
