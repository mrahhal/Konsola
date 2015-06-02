//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Attributes;
using Konsola.Attributes.Constraints;
using Xunit;

namespace Konsola.Tests
{
	public class CommandLineParserTests
	{
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

		[Fact(DisplayName = "Parsing incorrect nested command throws")]
		public void ParsingIncorrectNestedCommandThrows()
		{
			var args = "restore --an restore-sub --some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidParameter);
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