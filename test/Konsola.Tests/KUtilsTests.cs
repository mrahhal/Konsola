using System;
using Xunit;

namespace Konsola.Tests
{
	public class KUtilsTests
	{
		[Fact]
		public void SplitCommandLineArgs()
		{
			var args = "-my some -s2 \"something -int 3\" --sw".SplitCommandLineArgs();

			Assert.True(args.Length == 5);
			Assert.True(args[0] == "-my");
			Assert.True(args[1] == "some");
			Assert.True(args[2] == "-s2");
			Assert.True(args[3] == "something -int 3");
			Assert.True(args[4] == "--sw");
		}

	}
}