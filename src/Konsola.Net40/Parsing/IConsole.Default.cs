using System;
using System.IO;

namespace Konsola.Parsing
{
	/// <summary>
	/// An IConsole implementation that writes to system console.
	/// </summary>
	public class DefaultConsole : IConsole
	{
		private readonly object _sync = new object();

		public DefaultConsole()
		{
		}

		public void Write(WriteKind kind, string value)
		{
			var color = _GetColorFromKind(kind);
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

		private ConsoleColor _GetColorFromKind(WriteKind kind)
		{
			switch (kind)
			{
				case WriteKind.Normal:
					return ConsoleColor.Gray;
				case WriteKind.Info:
					return ConsoleColor.Blue;
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