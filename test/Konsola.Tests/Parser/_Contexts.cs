using System;

namespace Konsola.Parser.Tests
{
	public enum Platform
	{
		None,
		Windows,
		Unix,
		Linux,
	}

	[Flags]
	public enum FlagsPlatform
	{
		None = 0,
		Windows = 1,
		Unix = 2,
		Linux = 4,
	}

	[ContextOptions(HandleEmptyInvocationAsHelp = true)]
	[DefaultCommand(typeof(DefaultCommand))]
	[IncludeCommands(typeof(RestoreCommand), typeof(PositionCommand))]
	public class ContextWithHandleInvocationOptionSet : ContextBase
	{
	}

	[ContextOptions(Description = "This is some kind of a program description v1.0.123")]
	[DefaultCommand(typeof(DefaultCommand))]
	[IncludeCommands(typeof(RestoreCommand), typeof(PositionCommand))]
	public class Context : ContextBase
	{
	}

	public class DefaultCommand : CommandBase<Context>
	{
		[Parameter("my")]
		public string SomeString { get; set; }

		[Parameter("s1,s2")]
		public string SomeString2 { get; set; }

		[Parameter("set-something,s-s")]
		public string SomeString3 { get; set; }

		[Parameter("int", IsMandatory = true)]
		public int SomeInt { get; set; }

		[Parameter("sw")]
		public bool SomeBool { get; set; }

		[Parameter("sa")]
		public string[] StringArray { get; set; }

		[Parameter("p")]
		public Platform Platform { get; set; }

		[Parameter("fp")]
		public FlagsPlatform FlagsPlatform { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[Command("restore", Description = "restores something from there")]
	[IncludeCommands(typeof(RestoreSubCommand))]
	public class RestoreCommand : CommandBase<Context>
	{
		[Parameter("an", Description = "specify another")]
		public bool Another { get; set; }

		[Parameter("p", Description = "specify the chosen platform")]
		public Platform Plaform { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[Command("restore-sub")]
	public class RestoreSubCommand : CommandBase<Context>
	{
		[Parameter("some")]
		public bool Some { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[Command("position")]
	public class PositionCommand : CommandBase<Context>
	{
		[Parameter("some")]
		public string Some { get; set; }

		[Parameter("first", Position = 1)]
		public string First { get; set; }

		[Parameter("second", Position = 2)]
		public string Second { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[DefaultCommand(typeof(FaultyContextDefaultCommand))]
	public class FaultyContext : ContextBase
	{
	}

	public class FaultyContextDefaultCommand : CommandBase<FaultyContext>
	{
		[Parameter("my not")]
		public string SomeString { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[DefaultCommand(typeof(ContextWithInvalidCharsForParamDefaultCommand))]
	public class ContextWithInvalidCharsForParams : ContextBase
	{
	}

	public class ContextWithInvalidCharsForParamDefaultCommand : CommandBase<ContextWithInvalidCharsForParams>
	{
		[Parameter("my, some")]
		public string SomeString { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[ContextOptions(InvokeMethods = true)]
	[DefaultCommand(typeof(ContextWithOnParsedMethodDefaultCommand))]
	public class ContextWithOnParsedMethod : ContextBase
	{
	}

	[ContextOptions(InvokeMethods = true)]
	[DefaultCommand(typeof(ContextWithOnParsedMethodThatThrowsACommandLineExceptionDefaultCommand))]
	public class ContextWithOnParsedMethodThatThrowsACommandLineException : ContextBase
	{
	}

	[ContextOptions]
	[DefaultCommand(typeof(ContextWithOnParsedMethodDefaultCommand))]
	public class ContextWithOnParsedMethod_NotSet : ContextBase
	{
	}

	public class ContextWithOnParsedMethodDefaultCommand : CommandBase
	{
		public bool OnParsedCalled { get; private set; }

		[OnParsed]
		public virtual void OnParsed()
		{
			OnParsedCalled = true;
		}

		public override void ExecuteCommand()
		{
		}
	}

	public class ContextWithOnParsedMethodThatThrowsACommandLineExceptionDefaultCommand : CommandBase
	{
		[OnParsed]
		public virtual void OnParsed()
		{
			throw new CommandLineException("an exception");
		}

		public override void ExecuteCommand()
		{
		}
	}

	[ContextOptions(InvokeMethods = true)]
	[DefaultCommand(typeof(ContextWithOnParsedMethodSubDefaultCommand))]
	public class ContextWithOnParsedMethodSub : ContextBase
	{
	}

	public class ContextWithOnParsedMethodSubDefaultCommand : ContextWithOnParsedMethodDefaultCommand
	{
		public bool OnParsedOverrideCalled { get; private set; }

		[OnParsed]
		public override void OnParsed()
		{
			base.OnParsed();
			OnParsedOverrideCalled = true;
		}

		public override void ExecuteCommand()
		{
		}
	}

	public abstract class BaseCommand : CommandBase
	{
		[Parameter("bn")]
		public string BaseName { get; set; }
	}

	public class SubCommand : BaseCommand
	{
		[Parameter("sn")]
		public string SubName { get; set; }


		public override void ExecuteCommand()
		{
		}
	}

	public class CommandWithDefaultValues : CommandBase
	{
		[Parameter("p1")]
		public int Prop1 { get; set; }

		[Parameter("p2", Default = "foo")]
		public string Prop2 { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	public class CommandWithInvalidDefaultValues : CommandBase
	{
		[Parameter("p1", Default = "foo")]
		public int Prop1 { get; set; }

		[Parameter("p2", Default = "foo")]
		public string Prop2 { get; set; }

		public override void ExecuteCommand()
		{
		}
	}
}