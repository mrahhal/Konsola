# Konsola
Provides services for command line tools.

## Parsing command line args

##### Simple Usage
```c#
class Context : KContextBase
{
	[KParameter("my")]
	public string SomeString { get; set; }
	
	[KParameter("some", isMandatory: true)]
	public int SomeInt { get; set; }
	
	[KParameter("sw")]
	public bool SomeBool { get; set; }
}
```

And then:
```c#
var context = KContext.Parse<Context>(args); // args = "-some 2 --sw"
Assert.IsTrue(context.SomeString == null);
Assert.IsTrue(context.SomeInt == 2);
Assert.IsTrue(context.SomeBool == true);
```

##### More
Check **Konsola.Tests** for more usage scenarios.
