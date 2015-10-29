# Konsola
Contains services for command line tools.

[![Build status](https://img.shields.io/appveyor/ci/mrahhal/konsola/master.svg)](https://ci.appveyor.com/project/mrahhal/konsola)
[![Nuget version](https://img.shields.io/nuget/v/Konsola.svg)](https://www.nuget.org/packages/Konsola)
[![Nuget downloads](https://img.shields.io/nuget/dt/Konsola.svg)](https://www.nuget.org/packages/Konsola)

Latest nuget package [here](https://www.nuget.org/packages/Konsola).

*Use cases and samples are coming for `v1.0`*

### CommandLineParser
Dynamically parses command line args based on strong typed objects.

##### Simplest example:
```c#
[DefaultCommand(typeof(DefaultCommand))]
class Context : ContextBase
{
}

class DefaultCommand : CommandBase
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

##### Manual validation or any other work after binding is done
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

### More details
CommandLineParser knows about the following types:
* string, string array (-[param name])
* int (-[param name])
* bool (--[param name])
* enums, flag enums (-[param name])

string arrays and flag enums can have multiple values seperated by a comma.
For example: `-fe linux,windows` where "fe" corresponds to a flags enum parameter.

Automatically detects and throws exceptions on command line errors. It also prints to an `IConsole` when generating help and error messages.

A help message is automatically handled and printed when `--h` or `--help` is passed as an arg.

#### A lot more stuff
There's a lot more (default command, multiple and nested commands, context options, error detection, automatic printing to an extensible console, automatic help printing, constraints, positional params, ...).
