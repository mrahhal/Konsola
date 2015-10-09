# Konsola
Contains services for command line tools.

Latest nuget package [here](https://www.nuget.org/packages/Konsola).

[![Build status](https://ci.appveyor.com/api/projects/status/xsc0a2iarj4a292b?svg=true)](https://ci.appveyor.com/project/mrahhal/konsola)

***Note**: Use cases and samples are coming for `v1.0`*

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
var context = CommandLineParser.Parse<Context>(args);
var command = context.Command as DefaultCommand;

Assert.True(command.SomeString == "some");
Assert.True(command.SomeInt == 3);
Assert.True(command.SomeBool == true);

command.ExecuteCommand(); // Execute the command.
```

### More details
CommandLineParser knows about the following types:
* string, string array (-[param name])
* int (-[param name])
* bool (--[param name])
* enums, flag enums (-[param name])

string arrays and flag enums can have multiple values seperated by a comma.
For example: `-fe linux,windows` where "fe" corresponds to a flags enum parameter.

And it automatically detects and throws exceptions on command line errors. It also prints to an `IConsole` when generating help and error messages.

A help message is automatically handled and printed when `--h` or `--help` is passed as an arg.

#### A lot more stuff
There's a lot more (default command, multiple and nested commands, context options, error detection, automatic printing to an extensible console, automatic help printing, constraints, positional params, ...).

Check the unit tests project for more.
