using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class HelpContextGeneratorTests
	{
		[Fact]
		public void Generate()
		{
			var generator = CreateGenerator();
			var c = generator.Generate(typeof(Context));

			Assert.NotNull(c);
			Assert.Equal("This is some kind of a program description v1.0.123", c.Options.Description);
			Assert.True(c.Parameters.Count() != 0);
			Assert.True(c.NestedCommands.Count() != 0);
		}

		[Fact]
		public void GenerateForCommand()
		{
			var generator = CreateGenerator();
			var c = generator.GenerateForCommand(typeof(RestoreCommand));

			Assert.NotNull(c);
			Assert.Equal("restores something from there", c.Attribute.Description);
			Assert.True(c.Parameters.Count() != 0);
			Assert.True(c.NestedCommands.Count() != 0);
		}

		[Fact]
		public void GenerateForCommand_OrdersParametersFromBaseFirst()
		{
			var generator = CreateGenerator();
			var c = generator.GenerateForCommand(typeof(SubCommand));

			Assert.Equal(2, c.Parameters.Count());
			Assert.Equal("BaseName", c.Parameters.ElementAt(0).PropertyInfo.Name);
			Assert.Equal("SubName", c.Parameters.ElementAt(1).PropertyInfo.Name);
		}

		private static HelpContextGenerator CreateGenerator()
		{
			return new HelpContextGenerator(new ParameterContextProvider(new DefaultTokenizer()));
		}
	}
}
