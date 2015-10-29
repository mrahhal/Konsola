using System;
using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class HelpContextGeneratorTests
	{
		[Fact]
		public void Generate()
		{
			var c = HelpContextGenerator.Generate(typeof(Context), new DefaultTokenizer());

			Assert.NotNull(c);
			Assert.Equal("This is some kind of a program description v1.0.123", c.Options.Description);
			Assert.True(c.Parameters.Count() != 0);
			Assert.True(c.NestedCommands.Count() != 0);
		}

		[Fact]
		public void GenerateForCommand()
		{
			var c = HelpContextGenerator.GenerateForCommand(typeof(RestoreCommand), new DefaultTokenizer());

			Assert.NotNull(c);
			Assert.Equal("restores something from there", c.Attribute.Description);
			Assert.True(c.Parameters.Count() != 0);
			Assert.True(c.NestedCommands.Count() != 0);
		}
	}
}