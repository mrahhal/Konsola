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

			args = "restore --an".SplitCommandLineArgs();

			context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.InnerContext != null);
			Assert.IsTrue(context.InnerContext is Context.RestoreContext);
			Assert.IsTrue(((Context.RestoreContext)context.InnerContext).Another == true);
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

		[KCommand("restore")]
		public class RestoreContext : KContextBase
		{
			[KParameter("an")]
			public bool Another { get; set; }
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
			return Utils.SplitCommandLineArgs(args).ToArray();
		}
	}
}