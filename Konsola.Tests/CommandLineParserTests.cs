//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Attributes;
using Xunit;

namespace Konsola.Tests
{
	public class CommandLineParserTests
	{
		[Fact(DisplayName = "Split command line args")]
		public void SplitCommandLineArgsTest()
		{
			var args = "-my some -s2 \"something -int 3\" --sw".SplitCommandLineArgs();

			Assert.True(args.Length == 5);
			Assert.True(args[0] == "-my");
			Assert.True(args[1] == "some");
			Assert.True(args[2] == "-s2");
			Assert.True(args[3] == "something -int 3");
			Assert.True(args[4] == "--sw");
		}

		[Fact(DisplayName= "Parsing basic")]
		public void BasicParsing()
		{
			var args = "-my some -s2 something -int 3 --sw -set-something hello".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True(command != null);
			Assert.True(command.SomeString == "some");
			Assert.True(command.SomeString2 == "something");
			Assert.True(command.SomeString3 == "hello");
			Assert.True(command.SomeInt == 3);
			Assert.True(command.SomeBool == true);
		}

		[Fact(DisplayName = "Parsing command")]
		public void ParsingCommand()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Another == true);
		}

		[Fact(DisplayName="Parsing with invalid int value throws")]
		public void ParsingWithInvalidIntValueThrows()
		{
			var args = "-int some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidValue);
		}

		[Fact(DisplayName = "Parsing invalid command throws")]
		public void ParsingInvalidCommandThrows()
		{
			var args = "invalidcommand --an".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidCommand);
		}

		[Fact(DisplayName = "Parsing enum")]
		public void ParsingEnum()
		{
			var args = "-my some -p windows -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True(command.Platform == Platform.Windows);
		}

		[Fact(DisplayName = "Parsing invalid enum value throws")]
		public void ParsingInvalidEnumValueThrows()
		{
			var args = "-my some -p some -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});
		}

		[Fact(DisplayName = "Parsing multiple enum values with enum throws")]
		public void ParsingMultipleEnumValuesWithEnumThrows()
		{
			var args = "-my some -p windows,unix -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});
		}

		[Fact(DisplayName = "Parsing flags enum")]
		public void ParsingFlagsEnum()
		{
			var args = "-my some -fp windows,linux -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True((command.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (command.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[Fact(DisplayName = "Parsing enum inside command")]
		public void ParsingEnumInsideCommand()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Plaform == Platform.Linux);
		}

		[Fact(DisplayName = "Parsing invalid flags enum value throws")]
		public void ParsingInvalidFlagsEnumValueThrows()
		{
			var args = "-my some -fp windows,some -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});
		}

		[Fact(DisplayName = "Parsing with invalid context throws")]
		public void ParsingWithInvalidContextThrows()
		{
			var args = "-my some".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
			{
				CommandLineParser.Parse<FaultyContext>(args);
			});
		}

		[Fact(DisplayName = "Parsing with missing mandatory param throws")]
		public void ParsingWithMissingMandatoryParamThrows()
		{
			var args = "-my some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.MissingParameter);
		}

		[Fact(DisplayName = "Parsing with missing data throws")]
		public void ParsingWithMissingDataThrows()
		{
			var args = "-my -s2 something -int 3 --sw".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.MissingValue && ex.Name == "-my");
		}

		[Fact(DisplayName = "Parsing string array")]
		public void ParsingStringArray()
		{
			var args = "-sa some1,some2 -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True(command.StringArray != null);
			Assert.True(command.StringArray.Length == 2);
			Assert.True(command.StringArray[0] == "some1");
			Assert.True(command.StringArray[1] == "some2");
		}

		[Fact(DisplayName = "Parsing with context with invalid chars for params throws")]
		public void ParsingWithContextWithInvalidCharsForParamsThrows()
		{
			var args = "-int 3".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
				{
					CommandLineParser.Parse<ContextWithInvalidCharsForParams>(args);
				});
		}

		[Fact(DisplayName = "Parsing nested command")]
		public void ParsingNestedCommand()
		{
			var args = "restore restore-sub --some".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreSubCommand;

			Assert.True(command != null);
			Assert.True(command.Some == true);
		}
	}

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

	[ContextOptions]
	[IncludeCommands(typeof(RestoreCommand))]
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

	[Command("restore")]
	[IncludeCommands(typeof(RestoreSubCommand))]
	public class RestoreCommand : CommandBase<Context>
	{
		[Parameter("an")]
		public bool Another { get; set; }

		[Parameter("p")]
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

	[DefaultCommand(typeof(ContextWihtInvalidCharsForParamDefaultCommand))]
	public class ContextWithInvalidCharsForParams : ContextBase
	{
	}

	public class ContextWihtInvalidCharsForParamDefaultCommand : CommandBase<ContextWithInvalidCharsForParams>
	{
		[Parameter("my,-some")]
		public string SomeString { get; set; }

		public override void ExecuteCommand()
		{
		}
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return KUtils.SplitCommandLineArgs(args).ToArray();
		}
	}
}