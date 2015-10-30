using System;
using Xunit;

namespace Konsola.Parser.Tests
{
	public class BinderTests
	{
		[Fact]
		public void Bind_WithEmptySources()
		{
			// Arrange
			var c = new BindingContext
			{
				Sources = new DataSource[0],
				Targets = new PropertyTarget[0],
			};

			// Act + Assert
			Binder.Bind(c);
		}

		[Fact]
		public void Bind_PrefersProvidedValuesAgainsDefaultValues()
		{
			// Arrange
			var command = new CommandWithDefaultValues();
			var parameters =
				new ParameterContextProvider(new DefaultTokenizer()).GetFor(typeof(CommandWithDefaultValues));
			var c = new BindingContext
			{
				Sources = new[] { new DataSource() { Kind = RawTokenKind.Option, Value = "bar", Identifier = "p2", FullIdentifier = "-p2" } },
				Targets = new[] { new PropertyTarget(command, parameters[0]), new PropertyTarget(command, parameters[1]) }
			};

			// Act
			Binder.Bind(c);

			// Assert
			Assert.Equal("bar", command.Prop2);
		}

		[Fact]
		public void Bind_ValueNotProvided_UsesDefaultValue()
		{
			// Arrange
			var command = new CommandWithDefaultValues();
			var parameters =
				new ParameterContextProvider(new DefaultTokenizer()).GetFor(typeof(CommandWithDefaultValues));
			var c = new BindingContext
			{
				Sources = new DataSource[0],
				Targets = new[] { new PropertyTarget(command, parameters[0]), new PropertyTarget(command, parameters[1]) }
			};

			// Act
			Binder.Bind(c);

			// Assert
			Assert.Equal("foo", command.Prop2);
		}

		[Fact]
		public void Bind_WithInvalidDefaultValue_ThrowsContextException()
		{
			// Arrange
			var command = new CommandWithInvalidDefaultValues();
			var parameters =
				new ParameterContextProvider(new DefaultTokenizer()).GetFor(typeof(CommandWithInvalidDefaultValues));
			var c = new BindingContext
			{
				Sources = new DataSource[0],
				Targets = new[] { new PropertyTarget(command, parameters[0]), new PropertyTarget(command, parameters[1]) }
			};

			// Act + Assert
			Assert.Throws<ContextException>(() =>
			{
				Binder.Bind(c);
			});
		}
	}
}