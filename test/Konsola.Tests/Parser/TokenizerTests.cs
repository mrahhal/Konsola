using System;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class TokenizerTests
	{
		[Fact]
		public void Process_HandlesNull()
		{
			// Arrange
			var args = default(string[]);
			var tokenizer = new DefaultTokenizer();

			// Act + Assert
			Assert.Throws<ArgumentNullException>(() =>
				{
					tokenizer.Process(args);
				});
		}

		[Fact]
		public void Process_HandlesEmptyArgs()
		{
			// Arrange
			var args = "".SplitCommandLineArgs();
			var tokenizer = new DefaultTokenizer();

			// Act
			var tokens = tokenizer.Process(args);

			// Assert
			Assert.NotNull(tokens);
			Assert.Equal(0, tokens.Length);
		}

		[Fact]
		public void Process_StartsWithOneCommand()
		{
			// Arrange
			var args = "somecommand -o foo --sw".SplitCommandLineArgs();
			var tokenizer = new DefaultTokenizer();

			// Act
			var tokens = tokenizer.Process(args);

			// Assert
			AssertTokens(new Token[] {
				new CommandToken("somecommand"),
				new OptionToken("-o"),
				new DataToken("foo"),
				new SwitchToken("--sw"),
			}, tokens);
		}

		[Fact]
		public void Process_StartsWithNestedCommands()
		{
			// Arrange
			var args = "somecommand anothercommand -o foo --sw".SplitCommandLineArgs();
			var tokenizer = new DefaultTokenizer();

			// Act
			var tokens = tokenizer.Process(args);

			// Assert
			AssertTokens(new Token[] {
				new CommandToken("somecommand"),
				new CommandToken("anothercommand"),
				new OptionToken("-o"),
				new DataToken("foo"),
				new SwitchToken("--sw"),
			}, tokens);
		}

		[Fact]
		public void Process_EndsWithExtraneousData()
		{
			// Arrange
			var args = "somecommand anothercommand -o foo --sw bar".SplitCommandLineArgs();
			var tokenizer = new DefaultTokenizer();

			// Act
			var tokens = tokenizer.Process(args);

			// Assert
			AssertTokens(new Token[] {
				new CommandToken("somecommand"),
				new CommandToken("anothercommand"),
				new OptionToken("-o"),
				new DataToken("foo"),
				new SwitchToken("--sw"),
				new DataToken("bar"),
			}, tokens);
		}

		[Fact]
		public void Process_StartsWithOption()
		{
			// Arrange
			var args = "-o foo --sw".SplitCommandLineArgs();
			var tokenizer = new DefaultTokenizer();

			// Act
			var tokens = tokenizer.Process(args);

			// Assert
			AssertTokens(new Token[] {
				new OptionToken("-o"),
				new DataToken("foo"),
				new SwitchToken("--sw"),
			}, tokens);
		}

		private void AssertTokens(Token[] expected, Token[] actual)
		{
			Assert.NotNull(actual);
			Assert.Equal(expected.Length, actual.Length);
			for (int i = 0; i < actual.Length; i++)
			{
				Assert.Equal(expected[i].Kind, actual[i].Kind);
				Assert.Equal(expected[i].Value, actual[i].Value);
			}
		}
	}
}