# Konsola
Contains services for command line tools.

[![Build status](https://img.shields.io/appveyor/ci/mrahhal/konsola/master.svg)](https://ci.appveyor.com/project/mrahhal/konsola)
[![Nuget version](https://img.shields.io/nuget/v/Konsola.svg)](https://www.nuget.org/packages/Konsola)
[![Nuget downloads](https://img.shields.io/nuget/dt/Konsola.svg)](https://www.nuget.org/packages/Konsola)

Latest nuget package [here](https://www.nuget.org/packages/Konsola).

### CommandLineParser
Dynamically parses command line args based on strong typed objects.

#### Features:
- Complex dynamic parsing
- Nested commands to support complex parsing requirements
- Automatic help generation
- Automatic erorr handling
- Positional parameters
- Customizable (eror handling, help printing, ...)
- Portable (there are some use cases in android apps)

#### Simplest example:
```c#
[DefaultCommand(typeof(DefaultCommand))]
class Context : ContextBase
{
}

class DefaultCommand : CommandBase<Context>
{
    [Parameter("m")]
    public string SomeString { get; set; }

    [Parameter("s")]
    public int SomeInt { get; set; }

    [Parameter("sw")]
    public bool SomeBoolean { get; set; }

    public override void ExecuteCommand()
    {
    }
}

// Some other place... (in Main for example)
/* var args = "-m some -s 3 --sw"; */
var parser = new CommandLineParser<Context>();
var result = parser.Parse(args);
if (result.Kind == ParsingResultKind.Success)
{
    var command = result.Context.Command as DefaultCommand;
    Assert.True(command.SomeString == "some");
    Assert.True(command.SomeInt == 3);
    Assert.True(command.SomeBool == true);

    command.ExecuteCommand(); // Execute the command.
}
```

#### Overview
Every operation that can be done should be encapsulated in a Command class extending `CommandBase`. This class will be the context of your program invocation. You write options and switches as normal properties in your `command` class and decorate each with a `Parameter` attribute that defines this parameter.

Konsola is then able to do many of the error prone operations (such as arguments parsing) perfectly and safely. In addition, Konsola offers a whole lot of features that will enable you to focus on your requirements instead of complex parsing requirements, help formatting, error handling...

#### Supported types
Konsola knows about:
- `string` + `string[]`
- `int`
- `bool`
- enums + flag enums

All of which will be considered options except for `bool` parameters which will be considered as switches. (Options have values whereas switches are `off` or `on`)

String arrays and flag enums can have multiple values seperated by a comma.
For example: `-fe linux,windows` where "fe" corresponds to a flags enum parameter.

#### Usage
Your component's (or program's) commands are put together by a class (usually called `Context`) extending `ContextBase` that you'll define. You'll then add attributes to include your commands.

```c#
[ContextOptions(Description = "The description of the program.")] // This is optional
[DefaultCommandAttribute(typeof(DefaultCommand))]
[IncludeCommandsAttribute(typeof(FirstCommand), typeof(SecondCommand))]
public class Context : ContextBase
{
}
```

`ContextOptionsAttribute` is used to configure some of the behaviors of the parser.

The default command is a special command (it does extend CommandBase though) that will be invoked by Konsola if no commands are specified on the command line (such as: `program.exe -p something`).

Every other command should be defined as follows:

```c#
[CommandAttribute("first")] // This is not optional for normal commands
public class FirstCommand : CommandBase<Context>
{
    // Properties
    ...
    
    public override void ExecuteCommand()
    {
    }
}
```

Commands can be nested to give the following behavior:
`program.exe some sub -p foo`
`some` is command that also includes a `sub` command using the `IncludeCommandsAttribue` just as we did on the context class.

#### Parsing
```c#
// Create the parser that will bind to the Context class we defined earlier
var parser = new CommandLineParser<Context>();
var result = parser.Parse(args);

// Success means the command has been bound.
// Handled means something like help printing has been done.
// Failure means the args are faulty and that the error message has been printed to the `IConsole`.
if (result.Kind == ParsingResultKind.Success)
{
    // Your logic will be in the ExecuteCommand method of each command.
    result.Context.Command.ExecuteCommand();
}
```

The whole operation is customizable as the `CommandLineParser` ctor can take several arguments some of which:
- An `IConsole` that handles the printing of help and error messages.
- An `IHelpFormatter` that formats the help message.
- An `IErrorFormatter` that formats the error message.
- You can even provide a custom `Tokenizer` that overrides some of the simpler properties like specifying that a "-" means an option and a "--" means a switch (you can change those).

#### Manual validation
You can use `OnParsedAttribute` to decorate a method on your command. This method will then be called after the command is bound. You can then do manual validation or any other work and throw `CommandLineException` to allow the normal error flow to handle it.

```c#
[Command("some")]
class SomeCommand : CommandBase
{
    [Parameter("m")]
    public string SomeString { get; set; }

    [OnParsed]
    public void ValidateOrSomething()
    {
        if (SomeString.Contains('='))
        {
            throw new CommandLineException("m contains invalid characters.");
        }
    }

    public override void ExecuteCommand()
    {
    }
}
```
