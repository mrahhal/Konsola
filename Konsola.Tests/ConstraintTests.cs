//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using Xunit;

namespace Konsola.Tests
{
	public class ConstraintTests
	{
		[Fact]
		public void Parse_WithConstraintViolation_Throws()
		{
			var args = "-some 103".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.Constraint);
		}

		[Theory]
		[InlineData(3)]
		[InlineData(99)]
		public void Range(string value)
		{
			var args = ("-some " + value).SplitCommandLineArgs();

			var context = CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			var command = context.Command as ConstraintsContextCommand;

			Assert.True(command.Some == int.Parse(value));
		}

		[Theory]
		[InlineData(2)]
		[InlineData(100)]
		[InlineData(102)]
		public void Range_WithInvalidValues_Throws(string value)
		{
			var args = ("-some " + value).SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.Constraint);
		}

		[Theory]
		[InlineData("5")]
		[InlineData("100")]
		public void Range_InclusiveMax(string value)
		{
			var args = ("-some2 " + value).SplitCommandLineArgs();

			var context = CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			var command = context.Command as ConstraintsContextCommand;

			Assert.True(command.Some2 == int.Parse(value));
		}

		[Theory]
		[InlineData("1")]
		[InlineData("101")]
		public void Range_WithInclusiveMaxWithInvalidValues_Throws(string value)
		{
			var args = ("-some2 " + value).SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.Constraint);
		}
	}
}