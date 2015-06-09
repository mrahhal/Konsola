# Konsola
Contains services for command line tools.

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
* enums (-[param name])

string arrays and flag enums can have multiple values seperated by a comma.
For example: "-fe linux,windows" where fe corresponds to a flags enum parameter.

And it automatically detects and throws exceptions on command line errors.

#### A lot more stuff
There's a lot more (nested commands, flag enums, context options, ...). Check the unit tests project for more.