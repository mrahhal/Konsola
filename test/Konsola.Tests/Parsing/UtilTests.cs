using System;
using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class UtilTests
	{
		[Fact]
		public void SplitCommandLineArgs()
		{
			var args = Util.SplitCommandLineArgs( "-my some -s2 \"something -int 3\" --sw").ToArray();

			Assert.True(args.Length == 5);
			Assert.True(args[0] == "-my");
			Assert.True(args[1] == "some");
			Assert.True(args[2] == "-s2");
			Assert.True(args[3] == "something -int 3");
			Assert.True(args[4] == "--sw");
		}
	}
}