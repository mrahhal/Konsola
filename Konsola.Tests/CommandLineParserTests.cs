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

			Assert.True(context.InnerContext == null);
			Assert.True(context.SomeString == "some");
			Assert.True(context.SomeString2 == "something");
			Assert.True(context.SomeString3 == "hello");
			Assert.True(context.SomeInt == 3);
			Assert.True(context.SomeBool == true);
		}

		[Fact(DisplayName = "Parsing command")]
		public void ParsingCommand()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);

			Assert.True(context.InnerContext != null);
			Assert.True(context.InnerContext is RestoreCommand);
			Assert.True(((RestoreCommand)context.InnerContext).Another == true);
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

			Assert.True(context.Platform == Platform.Windows);
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

			Assert.True((context.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (context.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[Fact(DisplayName = "Parsing enum inside command")]
		public void ParsingEnumInsideCommand()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);

			Assert.True(context.InnerContext != null);
			Assert.True(context.InnerContext is RestoreCommand);
			Assert.True(((RestoreCommand)context.InnerContext).Plaform == Platform.Linux);
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

			Assert.True(context.StringArray != null);
			Assert.True(context.StringArray.Length == 2);
			Assert.True(context.StringArray[0] == "some1");
			Assert.True(context.StringArray[1] == "some2");
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

			Assert.True(context.InnerContext is RestoreCommand);
			Assert.True(context.InnerContext.InnerContext is RestoreSubCommand);
			Assert.True((context.InnerContext.InnerContext as RestoreSubCommand).Some == true);
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
	public class Context : ContextBase
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
	}

	[Command("restore")]
	[IncludeCommands(typeof(RestoreSubCommand))]
	public class RestoreCommand : CommandBase
	{
		[Parameter("an")]
		public bool Another { get; set; }

		[Parameter("p")]
		public Platform Plaform { get; set; }
	}

	[Command("restore-sub")]
	public class RestoreSubCommand : CommandBase
	{
		[Parameter("some")]
		public bool Some { get; set; }
	}

	[ContextOptions]
	public class FaultyContext : ContextBase
	{
		[Parameter("my not")]
		public string SomeString { get; set; }
	}

	public class ContextWithInvalidCharsForParams : ContextBase
	{
		[Parameter("my,-some")]
		public string SomeString { get; set; }
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return KUtils.SplitCommandLineArgs(args).ToArray();
		}
	}
}