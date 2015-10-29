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
	}
}