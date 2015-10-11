using System;
using Konsola.Parsing.Constraints;

namespace Konsola.Parsing.Tests
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

	[ContextOptions(Description = "This is some kind of a program description v1.0.123")]
	[IncludeCommands(typeof(RestoreCommand), typeof(PositionCommand))]
	[DefaultCommand(typeof(DefaultCommand))]
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

		[Parameter("sa")]
		public string[] StringArray { get; set; }

		[Parameter("int", IsMandatory = true)]
		public int SomeInt { get; set; }

		[Parameter("sw")]
		public bool SomeBool { get; set; }

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
		[Parameter("my,-some")]
		public string SomeString { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	[DefaultCommand(typeof(ConstraintsContextCommand))]
	public class ConstraintsContext : ContextBase
	{
	}

	public class ConstraintsContextCommand : CommandBase<ConstraintsContext>
	{
		[Parameter("some")]
		[Range(3, 100, IsMaxInclusive = false)]
		public int Some { get; set; }

		[Parameter("some2")]
		[Range(3, 100, IsMaxInclusive = true)]
		public int Some2 { get; set; }

		public override void ExecuteCommand()
		{
		}
	}
}