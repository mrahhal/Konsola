using System;
using System.Linq;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class CommandLineParserTests
	{
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return Util.SplitCommandLineArgs(args).ToArray();
		}
	}
}