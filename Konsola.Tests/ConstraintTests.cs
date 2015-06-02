//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using Xunit;

namespace Konsola.Tests
{
	public class ConstraintTests
	{
		[Fact(DisplayName = "Parsing with constraint violation throws")]
		public void ParsingWithConstraintViolationThrows()
		{
			var args = "-some 103".SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.Constraint);
		}

		[Theory(DisplayName = "Range basic test")]
		[InlineData(3)]
		[InlineData(99)]
		public void RangeTest(string value)
		{
			var args = ("-some " + value).SplitCommandLineArgs();

			var context = CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			var command = context.Command as ConstraintsContextCommand;

			Assert.True(command.Some == int.Parse(value));
		}

		[Theory(DisplayName = "Range with invalid values throws")]
		[InlineData(2)]
		[InlineData(100)]
		[InlineData(102)]
		public void RangeWithInvalidValuesThrows(string value)
		{
			var args = ("-some " + value).SplitCommandLineArgs();

			var ex = Assert.Throws<CommandLineException>(() =>
			{
				CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			});

			Assert.True(ex.Kind == CommandLineExceptionKind.Constraint);
		}

		[Theory(DisplayName = "Range with inclusive max")]
		[InlineData("5")]
		[InlineData("100")]
		public void RangeWithInclusiveMax(string value)
		{
			var args = ("-some2 " + value).SplitCommandLineArgs();

			var context = CommandLineParser.Parse<ConstraintsContext>(args, Console.Silent);
			var command = context.Command as ConstraintsContextCommand;

			Assert.True(command.Some2 == int.Parse(value));
		}

		[Theory(DisplayName = "Range with inclusive max with invalid values throws")]
		[InlineData("1")]
		[InlineData("101")]
		public void RangeWithInclusiveMaxWithInvalidValuesThrows(string value)
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