//I created a singular class to init my commands
//and all commands are defined in this same file.
//You may define all your commands in different files
//like a sane person but I'm not a sane person.

using Kraken.DevCon;
using UnityEngine;
using UnityEngine.Rendering;

//This class will init all my commands/console vars
public class DemoLib
{

    public DemoLib()
    {
        if(DeveloperConsoleAPI.bIsInitialized) //check if initialized
        {
            DeveloperConsoleAPI.RegisterCommand(new PrintBullshit());
            DeveloperConsoleAPI.RegisterCommand(new TogglePostProcessing());
            DeveloperConsoleAPI.RegisterCommand(new SetFOV());

            DeveloperConsoleAPI.RegisterCVar<float>("ballSpeed", 1.0f);
            DeveloperConsoleAPI.RegisterCVar<string>("ballColor", "FF000000");
        }
    }
}

public class PrintBullshit : ConsoleCommand
{
    public PrintBullshit()
    {
        _command = "PrintBS"; //make sure you don't have any spaces
    }

    public override ConsoleOutput ProcessCommand(string[] args)
    {
        return new ConsoleOutput(ConsoleOutput.Type.INF, "This is bullshit!");
    }
}

public class TogglePostProcessing : ConsoleCommand
{
    public TogglePostProcessing()
    {
        _command = "PostProcess";
    }

    public override ConsoleOutput ProcessCommand(string[] args)
    {
        ConsoleOutput output = new ConsoleOutput(ConsoleOutput.Type.INF, "null");
        if (args[0].ToLower() == "on")
        {
            GameObject.FindObjectOfType<Volume>().gameObject.SetActive(true);
            output.message = "PostProcessing turned on!";
            return output;
        }
        if(args[0].ToLower() == "off")
        {
            GameObject.FindObjectOfType<Volume>().gameObject.SetActive(false);
            output.message = "PostProcessing turned off!";
            return output;
        }

        output.type = ConsoleOutput.Type.ERR;
        output.message = "Invalid arguments passed!";

        return output;
    }
}

//Probably a better idea to make this into console variable but this is just for example's sake
public class SetFOV: ConsoleCommand
{
    private int min_fov = 60;
    private int max_fov = 120;
    public SetFOV()
    {
        _command = "set_fov";
    }

    public override ConsoleOutput ProcessCommand(string[] args)
    {
        int input_fov = int.Parse(args[0]);

        input_fov = Mathf.Clamp(input_fov, min_fov, max_fov);

        var cams = GameObject.FindObjectsOfType<Camera>();

        foreach(Camera cam in cams)
        {
            cam.fieldOfView = input_fov;
        }

        return new ConsoleOutput(ConsoleOutput.Type.INF, "FOV = " + input_fov.ToString());
    }
}
