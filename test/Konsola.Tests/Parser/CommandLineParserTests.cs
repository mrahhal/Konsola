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
		public void Parse_WithNonExistentCommand()
		{
			// Arrange
			var args = "somecommand --sw".SplitCommandLineArgs();
			var parser = new CommandLineParser<Context>();

			// Act
			var result = parser.Parse(args);

			// Assert
			Assert.Equal(ParsingResultKind.Handled, result.Kind);
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