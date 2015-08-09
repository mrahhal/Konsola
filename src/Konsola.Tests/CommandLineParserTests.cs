//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Internal;
using Xunit;

namespace Konsola.Tests
{
	public class CommandLineParserTests
	{
		[Fact]
		public void Parse_Basic()
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

		[Fact]
		public void Parse_Command()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Another == true);
		}

		[Fact]
		public void Parse_WithInvalidIntValue_Throws()
		{
			var args = "-int some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidValue);
		}

		[Fact]
		public void Parse_WithInvalidCommand_Throws()
		{
			var args = "invalidcommand --an".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidCommand
				|| ex.Kind == CommandLineExceptionKind.InvalidParameter);
		}

		[Fact]
		public void Parse_Enum()
		{
			var args = "-my some -p windows -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True(command.Platform == Platform.Windows);
		}

		[Fact]
		public void Parse_WithInvalidEnumValue_Throws()
		{
			var args = "-my some -p some -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});
		}

		[Fact]
		public void Parse_WithMultipleEnumValuesWithEnum_Throws()
		{
			var args = "-my some -p windows,unix -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});
		}

		[Fact]
		public void Parse_FlagsEnum()
		{
			var args = "-my some -fp windows,linux -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True((command.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (command.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[Fact]
		public void Parse_EnumInsideCommand()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Plaform == Platform.Linux);
		}

		[Fact]
		public void Parse_WithInvalidFlagsEnumValue_Throws()
		{
			var args = "-my some -fp windows,some -int 3".SplitCommandLineArgs();

			Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});
		}

		[Fact]
		public void Parse_WithInvalidContext_Throws()
		{
			var args = "-my some".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
			{
				CommandLineParser.Parse<FaultyContext>(args);
			});
		}

		[Fact]
		public void Parse_WithMissingMandatoryParam_Throws()
		{
			var args = "-my some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.MissingParameter);
		}

		[Fact]
		public void Parse_WithMissingData_Throws()
		{
			var args = "-my -s2 something -int 3 --sw".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<Context>(args);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.MissingValue && ex.Name == "-my");
		}

		[Fact]
		public void Parse_StringArray()
		{
			var args = "-sa some1,some2 -int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as DefaultCommand;

			Assert.True(command.StringArray != null);
			Assert.True(command.StringArray.Length == 2);
			Assert.True(command.StringArray[0] == "some1");
			Assert.True(command.StringArray[1] == "some2");
		}

		[Fact]
		public void Parse_WithContextWithInvalidCharsForParams_Throws()
		{
			var args = "-int 3".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
				{
					CommandLineParser.Parse<ContextWithInvalidCharsForParams>(args);
				});
		}

		[Fact]
		public void Parse_NestedCommand()
		{
			var args = "restore restore-sub --some".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as RestoreSubCommand;

			Assert.True(command != null);
			Assert.True(command.Some == true);
		}

		[Fact]
		public void Parse_WithIncorrectNestedCommand_Throws()
		{
			var args = "restore --an restore-sub --some".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});

			Assert.True(ex.Kind == CommandLineExceptionKind.InvalidParameter);
		}

		[Fact]
		public void Parse_WithNoValuesForCommand_Throws()
		{
			var args = "restore".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					CommandLineParser.Parse<Context>(args);
				});

			Assert.True(ex.Kind == CommandLineExceptionKind.Invalid);
		}

		[Fact]
		public void Parse_WithHelp_TestConsole()
		{
			var args = "--h".SplitCommandLineArgs();

			var console = new TestConsole();
			var context = CommandLineParser.Parse<Context>(args, console);

			Assert.Null(context);
			Assert.True(!string.IsNullOrWhiteSpace(console.Text));
		}

		[Fact]
		public void Parse_WithHelp_ReturnsNull()
		{
			var args = "restore --an --h".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);

			Assert.Null(context);
		}

		[Fact]
		public void HelpInfo_Default()
		{
			var args = "-int 3".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var helpInfo = new HelpInfo(context.Command);

			Assert.True(helpInfo.ProgramDescription != null);
			Assert.True(helpInfo.Commands != null);
			Assert.True(helpInfo.Parameters != null);
		}

		[Fact]
		public void HelpInfo_OfCommand()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var helpInfo = new HelpInfo(context.Command);

			Assert.True(helpInfo.ProgramDescription == null);
			Assert.True(helpInfo.Commands != null);
			Assert.True(helpInfo.Parameters != null);
		}

		[Fact]
		public void Parse_WithPositionalParams()
		{
			var args = "position -some somestr fstr sstr".SplitCommandLineArgs();

			var context = CommandLineParser.Parse<Context>(args);
			var command = context.Command as PositionCommand;

			Assert.True(command.First == "fstr");
			Assert.True(command.Second == "sstr");
			Assert.True(command.Some == "somestr");
		}

		[Fact]
		public void Parse_WithInvalidPostionalParams_Throws()
		{
			var args = "position -some somestr asd fstr sstr".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
				{
					var context = CommandLineParser.Parse<Context>(args);
				});
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