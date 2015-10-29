using System;
using System.IO;

namespace Konsola.Parser
{
	/// <summary>
	/// An <see cref="IConsole"/> implementation that writes to system console.
	/// </summary>
	public class DefaultConsole : IConsole
	{
		private readonly object _sync = new object();

		public DefaultConsole()
		{
		}

		public void Write(WriteKind kind, string value)
		{
			var color = GetColorFromKind(kind);
			lock (_sync)
			{
				Console.ForegroundColor = color;
				try
				{
					Console.Write(value);
				}
				finally
				{
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
		}

		protected virtual ConsoleColor GetColorFromKind(WriteKind kind)
		{
			switch (kind)
			{
				case WriteKind.Success:
					return ConsoleColor.Green;
				case WriteKind.Info:
					return ConsoleColor.DarkGray;
				case WriteKind.Warning:
					return ConsoleColor.Yellow;
				case WriteKind.Error:
					return ConsoleColor.Red;
				default:
					return ConsoleColor.Gray;
			}
		}
	}
}