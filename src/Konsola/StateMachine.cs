using System;

namespace Konsola
{
	internal class StateMachine<T>
		where T : class
	{
		public StateMachine(T[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Data = data;
			Position = -1;
		}

		public T[] Data { get; }

		public int Position { get; set; }

		public bool HasNext => Position + 1 < Data.Length;

		public bool HasPrevious => Position - 1 >= 0;

		public T Current
		{
			get
			{
				if (Position >= 0 && Position < Data.Length)
				{
					return Data[Position];
				}
				return null;
			}
		}

		public void Reset()
		{
			Position = -1;
		}

		public T PeekNext()
		{
			var next = Position + 1;
			if (!HasNext)
			{
				return null;
			}
			return Data[next];
		}

		public T PeekPrevious()
		{
			var previous = Position - 1;
			if (!HasPrevious)
			{
				return null;
			}
			return Data[previous];
		}

		public T Next()
		{
			if (!HasNext)
			{
				return null;
			}
			return Data[++Position];
		}

		public T Previous()
		{
			if (!HasPrevious)
			{
				return null;
			}
			return Data[--Position];
		}

		public void VisitAllNext(Func<int, T, T> visit)
		{
			if (visit == null)
			{
				throw new ArgumentNullException(nameof(visit));
			}
			if (!HasNext)
			{
				return;
			}
			for (int i = Position + 1; i < Data.Length; i++)
			{
				var currentT = Data[i];
				var newT = visit(i, currentT);
				if (newT != currentT)
				{
					Data[i] = newT;
				}
			}
		}

		public void VisitAllPrevious(Func<int, T, T> visit)
		{
			if (visit == null)
			{
				throw new ArgumentNullException(nameof(visit));
			}
			if (!HasPrevious)
			{
				return;
			}
			for (int i = Position - 1; i >= 0; i--)
			{
				var currentT = Data[i];
				var newT = visit(i, currentT);
				if (newT != currentT)
				{
					Data[i] = newT;
				}
			}
		}
	}
}
