# Kraken-Developer-Console
 Developer Console for Unity
 
 ---

## Installation
 - Grab the package from the releases section of this repo.
 - Import it into your project as you would any normal unity package.

## Usage

### Seting up console
 - From the top menu select Kraken > Developer Console > Create Console Object
 - In any one of your scripts add a way to toggle the console. Use 
    > DeveloperConsoleAPI.ToggleConsole();
 - Change any settings you want in the DeveloperConsole GameObject Inspector

 Note:
 - DeveloperConsole is a DDOL(DontDestroyOnLoad) object. Create only once in a startup/bootstrapper scene.
 - If you directly start with a main menu, more than one Objects may be created after loading menu again. You can check for this and delete all but one instance if need be.
### Working with Console Variables

Console variables can be used to store values using the devconsole. These values can be retrieved anywhere/anytime. You can also add listeners to OnValueChanged to notify dependant systems of the value change.

```C#
DeveloperConsoleAPI.RegisterCvar<type>(cvar_name, default_value); //registering console variable
//type is the type of variable you're creating
//cvar_name is the identifier for your cvar (spaces are not allowed)
//default_value is the value you want it to initialize it to
//returns ConsoleVariable<T> if you would like to store a reference to it.

var cvar = DeveloperConsoleAPI.GetCvar<type>(cvar_name); //retrieves the console variable of given type
//returns ConsoleVariable<T>

cvar.value = new_val; //changing value of the cvar. Invokes OnValueChanged.

cvar.OnValueChanged.AddListener(YourFunction) //Invoked everytime the value is changed. UnityEvent<T> gives the new value as param

//example listener
public void YourFunction(float value)
{
    //Do stuff
}
```
Note:
 - Do not create cvars to types which cannot be converted using a TypeDescriptor
 - Spaces are not allowed in the variable identifier

### Working with Console Commands

Creating a new Console Command

```C#
public class MyConsoleCommand : ConsoleCommand //Available in Kraken.DevCon namespace
{
    public MyConsoleCommand //constructor required to give name to the command
    {
        _command = "myComm";
    }

    public override ConsoleOutput ProcessCommand(string[] args) //logic to process the command goes here. Returns a ConsoleOutput
    {
        ConsoleOutput output =  new ConsoleOutput(ConsoleOutput.Type.INF), "lolololol");

        //Do stuff

        return output
    }
}
```

Registering a Console Command

```C#
DeveloperConsoleAPI.RegisterCommand(new MyConsoleCommand()); //returns a bool 
//true -> registration successful
//false -> registration failed/command already exists
```

Any other guidance can be found in the Demo.

---

Contact me before creating a pull request. Leave a star on the repo if this helped you. 


