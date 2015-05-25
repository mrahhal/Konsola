//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Konsola.Tests
{
	[TestClass]
	public class ParsingTests
	{
		[TestMethod]
		public void SplitCommandLineArgsTest()
		{
			var args = "-my some -s2 \"something -int 3\" --sw".SplitCommandLineArgs();

			Assert.IsTrue(args.Length == 5);
			Assert.IsTrue(args[0] == "-my");
			Assert.IsTrue(args[1] == "some");
			Assert.IsTrue(args[2] == "-s2");
			Assert.IsTrue(args[3] == "something -int 3");
			Assert.IsTrue(args[4] == "--sw");
		}

		[TestMethod]
		public void BasicTest()
		{
			var args = "-my some -s2 something -int 3 --sw".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.InnerContext == null);
			Assert.IsTrue(context.SomeString == "some");
			Assert.IsTrue(context.SomeString2 == "something");
			Assert.IsTrue(context.SomeInt == 3);
			Assert.IsTrue(context.SomeBool == true);
		}

		[TestMethod]
		public void CommandBasicTest()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.InnerContext != null);
			Assert.IsTrue(context.InnerContext is Context.RestoreContext);
			Assert.IsTrue(((Context.RestoreContext)context.InnerContext).Another == true);
		}

		[TestMethod]
		public void EnumTest()
		{
			var args = "-my some -p windows -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.Platform == Platform.Windows);
		}

		[TestMethod, ExpectedException(typeof(ParsingException))]
		public void InvalidEnumValueShouldFail()
		{
			var args = "-my some -p some -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);
		}

		[TestMethod, ExpectedException(typeof(ParsingException))]
		public void MultipleEnumValuesWithEnumShouldFail()
		{
			var args = "-my some -p windows,unix -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);
		}

		[TestMethod]
		public void FlagsEnumTest()
		{
			var args = "-my some -fp windows,linux -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue((context.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (context.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[TestMethod]
		public void EnumInsideCommandTest()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.InnerContext != null);
			Assert.IsTrue(context.InnerContext is Context.RestoreContext);
			Assert.IsTrue(((Context.RestoreContext)context.InnerContext).Plaform == Platform.Linux);
		}

		[TestMethod, ExpectedException(typeof(ParsingException))]
		public void InvalidFlagsEnumValueShouldFail()
		{
			var args = "-my some -fp windows,some -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);
		}

		[TestMethod, ExpectedException(typeof(ContextException))]
		public void ShouldThrowIfContextNotValid()
		{
			var args = "-my some".SplitCommandLineArgs();

			var context = KContext.Parse<FaultyContext>(args);
		}

		[TestMethod, ExpectedException(typeof(ParsingException))]
		public void ShouldThrowIfMandantoryParamIsMissing()
		{
			var args = "-my some".SplitCommandLineArgs();

			try
			{
				var context = KContext.Parse<Context>(args);
			}
			catch (ParsingException ex)
			{
				Assert.IsTrue(ex.Kind == ExceptionKind.MissingParameter);
				throw;
			}
		}

		[TestMethod, ExpectedException(typeof(ParsingException))]
		public void ShouldThrowIfDataIsMissing()
		{
			var args = "-my -s2 something -int 3 --sw".SplitCommandLineArgs();

			try
			{
				var context = KContext.Parse<Context>(args);
			}
			catch (ParsingException ex)
			{
				Assert.IsTrue(ex.Kind == ExceptionKind.MissingValue && ex.Name == "-my");
				throw;
			}
		}
	}

	public enum Platform
	{
		None,
		Windows,
		Unix,
		Linux,
	}

	[Flags]
	public enum FlagsPlatform
	{
		None = 0,
		Windows = 1,
		Unix = 2,
		Linux = 4,
	}

	[KContextOptions]
	public class Context : KContextBase
	{
		[KParameter("my")]
		public string SomeString { get; set; }

		[KParameter("s1,s2")]
		public string SomeString2 { get; set; }

		[KParameter("int", isMandatory: true)]
		public int SomeInt { get; set; }

		[KParameter("sw")]
		public bool SomeBool { get; set; }

		[KParameter("p")]
		public Platform Platform { get; set; }

		[KParameter("fp")]
		public FlagsPlatform FlagsPlatform { get; set; }

		[KCommand("restore")]
		public class RestoreContext : KContextBase
		{
			[KParameter("an")]
			public bool Another { get; set; }

			[KParameter("p")]
			public Platform Plaform { get; set; }
		}
	}

	[KContextOptions]
	public class FaultyContext : KContextBase
	{
		[KParameter("my not")]
		public string SomeString { get; set; }
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return KUtils.SplitCommandLineArgs(args).ToArray();
		}
	}
}