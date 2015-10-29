using System;
using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class UtilTests
	{
		[Fact]
		public void SplitCommandLineArgs_HandlesNull()
		{
			// Arrange
			var raw = default(string);

			// Act + Assert
			Assert.Throws<ArgumentNullException>(() =>
				{
					Util.SplitCommandLineArgs(raw);
				});
		}

		[Fact]
		public void SplitCommandLineArgs_HandlesEmptyString()
		{
			// Arrange
			var raw = "";

			// Act
			var args = Util.SplitCommandLineArgs(raw).ToArray();

			// Assert
			Assert.NotNull(args);
			Assert.Equal(0, args.Length);
		}

		[Fact]
		public void SplitCommandLineArgs()
		{
			// Arrange
			var raw = "-my some -s2 \"something -int 3\" --sw";

			// Act
			var args = Util.SplitCommandLineArgs(raw).ToArray();

			// Assert
			Assert.True(args.Length == 5);
			Assert.True(args[0] == "-my");
			Assert.True(args[1] == "some");
			Assert.True(args[2] == "-s2");
			Assert.True(args[3] == "something -int 3");
			Assert.True(args[4] == "--sw");
		}
	}
}