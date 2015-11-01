using System;
using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class CommandLineParserTests
	{
		[Fact]
		public void Parse_Empty_WithHandleEmptyInvocationOptionSet_ShouldHandleHelp()
		{
			// Arrange
			var args = "".SplitCommandLineArgs();
			var parser = new CommandLineParser<ContextWithHandleInvocationOptionSet>();

			// Act
			var result = parser.Parse(args);

			// Assert
			Assert.Equal(ParsingResultKind.Handled, result.Kind);
		}

		[Fact]
		public void Parse_Empty_WithHandleEmptyInvocationOptionNotSet_ShouldNotHandleHelp()
		{
			// Arrange
			var args = "-int 42".SplitCommandLineArgs();
			var parser = new CommandLineParser<Context>();

			// Act
			var result = parser.Parse(args);

			// Assert
			Assert.Equal(ParsingResultKind.Success, result.Kind);
		}

		[Fact]
		public void Parse_WithInvalidCommand()
		{
			// Arrange
			var args = "somecommand --sw".SplitCommandLineArgs();
			var parser = new CommandLineParser<Context>();

			// Act
			var result = parser.Parse(args);

			// Assert
			Assert.Equal(ParsingResultKind.Handled, result.Kind);
		}

		[Fact]
		public void Parse_Basic()
		{
			var args = "-my some -s2 something -int 3 -sa first,second --sw -set-something hello".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as DefaultCommand;

			Assert.True(result.Kind == ParsingResultKind.Success);
			Assert.True(command.SomeString == "some");
			Assert.True(command.SomeString2 == "something");
			Assert.True(command.SomeString3 == "hello");
			Assert.True(command.SomeInt == 3);
			Assert.True(command.SomeBool == true);
			Assert.True(command.StringArray.Length == 2);
			Assert.True(command.StringArray[0] == "first");
			Assert.True(command.StringArray[1] == "second");
		}

		[Fact]
		public void Parse_IgnoresWhiteSpaceForStringArrays()
		{
			var args = "-int 3 -sa ,first,,second,".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as DefaultCommand;

			Assert.True(command.StringArray.Length == 2);
			Assert.True(command.StringArray[0] == "first");
			Assert.True(command.StringArray[1] == "second");
		}

		[Fact]
		public void Parse_InvokesOnParsedMethods_WhenInvokeMethodsIsSet()
		{
			var args = "".SplitCommandLineArgs();

			var parser = new CommandLineParser<ContextWithOnParsedMethod>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as ContextWithOnParsedMethodDefaultCommand;

			Assert.True(command.OnParsedCalled);
		}

		[Fact]
		public void Parse_DoesNotInvokeOnParsedMethods_WhenInvokeMethodsIsNotSet()
		{
			var args = "".SplitCommandLineArgs();

			var parser = new CommandLineParser<ContextWithOnParsedMethod_NotSet>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as ContextWithOnParsedMethodDefaultCommand;

			Assert.False(command.OnParsedCalled);
		}

		[Fact]
		public void Parse_CommandLineExceptionsAreCorrectlyHandledWithinOnParsedMethods()
		{
			var args = "".SplitCommandLineArgs();

			var parser = new CommandLineParser<ContextWithOnParsedMethodThatThrowsACommandLineException>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_OnParsedMethods_PlaysWellWithVirtual()
		{
			var args = "".SplitCommandLineArgs();

			var parser = new CommandLineParser<ContextWithOnParsedMethodSub>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as ContextWithOnParsedMethodSubDefaultCommand;

			Assert.True(command.OnParsedCalled);
			Assert.True(command.OnParsedOverrideCalled);
		}

		[Fact]
		public void Parse_Command()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Another == true);
		}

		[Fact]
		public void Parse_WithInvalidIntValue_Throws()
		{
			var args = "-int some".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_Enum()
		{
			var args = "-my some -p windows -int 3".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as DefaultCommand;

			Assert.True(command.Platform == Platform.Windows);
		}

		[Fact]
		public void Parse_WithInvalidEnumValue_Throws()
		{
			var args = "-my some -p some -int 3".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_WithMultipleEnumValuesWithEnum_Throws()
		{
			var args = "-my some -p windows,unix -int 3".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_MixedValuesBug_WorkItem15()
		{
			var args = "wi15 -e u -data foo".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_FlagsEnum()
		{
			var args = "-my some -fp windows,linux -int 3".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as DefaultCommand;

			Assert.True((command.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (command.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[Fact]
		public void Parse_EnumInsideCommand()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as RestoreCommand;

			Assert.True(command != null);
			Assert.True(command.Plaform == Platform.Linux);
		}

		[Fact]
		public void Parse_WithInvalidFlagsEnumValue_Throws()
		{
			var args = "-my some -fp windows,some -int 3".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_WithInvalidContext_Throws()
		{
			var args = "-my some".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
			{
				var parser = new CommandLineParser<FaultyContext>();
				parser.Parse(args);
			});
		}

		[Fact]
		public void Parse_WithMissingMandatoryParam_Throws()
		{
			var args = "-my some".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_WithMissingData_Throws()
		{
			var args = "-my -s2 something -int 3 --sw".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}

		[Fact]
		public void Parse_WithContextWithInvalidCharsForParams_Throws()
		{
			var args = "-int 3".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
			{
				var parser = new CommandLineParser<ContextWithInvalidCharsForParams>();
				parser.Parse(args);
			});
		}

		[Fact]
		public void Parse_NestedCommand()
		{
			var args = "restore restore-sub --some".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as RestoreSubCommand;

			Assert.True(command != null);
			Assert.True(command.Some == true);
		}

		[Fact]
		public void Parse_WithIncorrectNestedCommand_Throws()
		{
			var args = "restore --an restore-sub --some".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
			Assert.Equal(CommandLineExceptionKind.InvalidParameter, result.Exception.Kind);
		}

		[Fact]
		public void Parse_WithNoValuesForCommand_DoesNotThrow()
		{
			var args = "restore".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Success, result.Kind);
		}

		[Fact]
		public void Parse_WithHelp_TestConsole()
		{
			var args = "--h".SplitCommandLineArgs();

			var console = new TestConsole();
			var parser = new CommandLineParser<Context>(console: console);
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Handled, result.Kind);
			Assert.True(!string.IsNullOrWhiteSpace(console.Text));
		}

		[Fact]
		public void Parse_WithHelp_ReturnsNull()
		{
			var args = "restore --an --h".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;

			Assert.Null(context);
		}

		[Fact]
		public void Parse_WithPositionalParams()
		{
			var args = "position fstr sstr -some somestr".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);
			var context = result.Context;
			var command = context.Command as PositionCommand;

			Assert.Equal("fstr", command.First);
			Assert.Equal("sstr", command.Second);
			Assert.Equal("somestr", command.Some);
		}

		[Fact]
		public void Parse_WithInvalidPostionalParams_Throws()
		{
			var args = "position -some somestr asd fstr sstr".SplitCommandLineArgs();

			var parser = new CommandLineParser<Context>();
			var result = parser.Parse(args);

			Assert.Equal(ParsingResultKind.Failure, result.Kind);
		}
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return Util.SplitCommandLineArgs(args).ToArray();
		}
	}
}