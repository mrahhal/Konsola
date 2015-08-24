//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.IO;

namespace Konsola
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

		public void WriteLine(WriteKind kind, string value)
		{
			Write(kind, value + Environment.NewLine);
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