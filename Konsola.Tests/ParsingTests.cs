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
		public void TestMethod1()
		{
			var args = "-my some -int 3 --sw".Split(' ');

			var context = KContext.Parse<Context>(args);

			Assert.IsTrue(context.MyProperty1 == "some");
			Assert.IsTrue(context.SomeInt1 == 3);
			Assert.IsTrue(context.SomeBool1 == true);
		}
	}

	[KClass]
	public class Context
	{
		[KParameter("my", ParameterKind.String)]
		public string MyProperty1 { get; set; }

		[KParameter("int", ParameterKind.Int, IsMandantory = true)]
		public int SomeInt1 { get; set; }

		[KParameter("sw", ParameterKind.Switch)]
		public bool SomeBool1 { get; set; }
	}
}