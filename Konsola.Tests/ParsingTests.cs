//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Konsola.Tests
{
	[TestClass]
	public class ParsingTests
	{
		[TestMethod]
		public void BasicTest()
		{
			var args = "-my some -s2 something -int 3 --sw".Split(' ');

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.SomeString == "some");
			Assert.IsTrue(context.SomeString2 == "something");
			Assert.IsTrue(context.SomeInt == 3);
			Assert.IsTrue(context.SomeBool == true);
		}

		[TestMethod]
		public void ShouldThrowIfContextNotValid()
		{
			var args = "-my some".Split(' ');

			try
			{
				var context = KContext.Parse<FaultyContext>(args);
				Assert.Fail();
			}
			catch (ContextException)
			{
			}
		}

		[TestMethod]
		public void ShouldThrowIfMandantoryParamIsMissing()
		{
			var args = "-my some".Split(' ');

			try
			{
				var context = KContext.Parse<Context>(args);
				Assert.Fail();
			}
			catch (ParsingException ex)
			{
				Assert.IsTrue(ex.Kind == ExceptionKind.MissingParameter);
			}
		}

		[TestMethod]
		public void ShouldThrowIfDataIsMissing()
		{
			var args = "-my -s2 something -int 3 --sw".Split(' ');

			try
			{
				var context = KContext.Parse<Context>(args);
			}
			catch (ParsingException ex)
			{
				Assert.IsTrue(ex.Kind == ExceptionKind.MissingData && ex.Name == "my");
			}
		}
	}

	[KClass]
	public class Context
	{
		[KParameter("my")]
		public string SomeString { get; set; }

		[KParameter("s1;s2")]
		public string SomeString2 { get; set; }

		[KParameter("int", IsMandantory = true)]
		public int SomeInt { get; set; }

		[KParameter("sw")]
		public bool SomeBool { get; set; }
	}

	[KClass]
	public class FaultyContext
	{
		[KParameter("my not")]
		public string SomeString { get; set; }
	}
}