using System;
using Xunit;

namespace Konsola
{
	public class StateMachineTests : IDisposable
	{
		[Fact]
		public void HandlesNull()
		{
			// Arrange + Act + Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				var sm = new StateMachine<Test>(null);
			});
		}

		[Fact]
		public void HasNext()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });

			// Act + Assert
			Assert.True(sm.HasNext);
		}

		[Fact]
		public void HasNext_Not()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 1;

			// Act + Assert
			Assert.False(sm.HasNext);
		}

		[Fact]
		public void HasPrevious()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 0;

			// Act + Assert
			Assert.True(sm.HasNext);
		}

		[Fact]
		public void HasPrevious_Not()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });

			// Act + Assert
			Assert.False(sm.HasPrevious);
		}

		[Fact]
		public void Position()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Next();

			// Act + Assert
			Assert.Equal(0, sm.Position);
		}

		[Fact]
		public void Reset()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Next();

			// Act
			sm.Reset();

			// Assert
			Assert.Equal(-1, sm.Position);
		}

		[Fact]
		public void PeekNext()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });

			// Act
			var t = sm.PeekNext();

			// Assert
			Assert.Equal(-1, sm.Position);
			Assert.Equal(0, t.Id);
		}

		[Fact]
		public void PeekPrevious()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 1;

			// Act
			var t = sm.PeekPrevious();

			// Assert
			Assert.Equal(1, sm.Position);
			Assert.Equal(0, t.Id);
		}

		[Fact]
		public void Current()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 0;

			// Act
			var t = sm.Current;

			// Assert
			Assert.NotNull(t);
		}

		[Fact]
		public void Current_Null()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });

			// Act
			var t = sm.Current;

			// Assert
			Assert.Null(t);
		}

		[Fact]
		public void VisitAllNext()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 0;

			// Act
			sm.VisitAllNext((i, t) =>
			{
				t.Deposit = string.Empty;
				return t;
			});

			// Assert
			Assert.Null(sm.Data[0].Deposit);
			Assert.NotNull(sm.Data[1].Deposit);
		}

		[Fact]
		public void VisitAllPrevious()
		{
			// Arrange
			var sm = new StateMachine<Test>(new[] { new Test(), new Test() });
			sm.Position = 1;

			// Act
			sm.VisitAllPrevious((i, t) =>
			{
				t.Deposit = string.Empty;
				return t;
			});

			// Assert
			Assert.NotNull(sm.Data[0].Deposit);
			Assert.Null(sm.Data[1].Deposit);
		}

		public void Dispose()
		{
			Test.Reset();
		}

		private class Test
		{
			private static int _counter = 0;

			public static void Reset()
			{
				_counter = 0;
			}

			public Test()
			{
				Id = _counter++;
			}

			public int Id { get; private set; }

			public string Deposit { get; set; }
		}
	}
}
