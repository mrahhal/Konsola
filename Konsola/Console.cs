//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.IO;
using SConsole = System.Console;

namespace Konsola
{
	/// <summary>
	/// An IConsole implementation that writes to system console.
	/// </summary>
	public class Console : IConsole
	{
		private readonly object _sync = new object();

		public Console()
		{
		}

		public int WindowWidth
		{
			get
			{
				try
				{
					var width = SConsole.WindowWidth;
					if (width > 0)
					{
						return width;
					}
					else
					{
						return int.MaxValue;
					}
				}
				catch (IOException)
				{
					return int.MaxValue;
				}
			}
		}

		public int CursorLeft
		{
			get
			{
				try
				{
					return SConsole.CursorLeft;
				}
				catch (IOException)
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets a console that silently consumes messages.
		/// </summary>
		public static IConsole Silent
		{
			get
			{
				return SilentConsole.Singleton;
			}
		}

		public void Write(WriteKind kind, string value)
		{
			var color = _GetColorFromKind(kind);
			lock (_sync)
			{
				SConsole.ForegroundColor = color;
				try
				{
					SConsole.Write(value);
				}
				finally
				{
					SConsole.ForegroundColor = ConsoleColor.Gray;
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

		private class SilentConsole : IConsole
		{
			public static readonly SilentConsole Singleton;

			static SilentConsole()
			{
				Singleton = new SilentConsole();
			}

			private SilentConsole()
			{
			}

			public int WindowWidth { get { return -1; } }
			public int CursorLeft { get { return -1; } }

			public void Write(WriteKind kind, string value) { }
			public void WriteLine(WriteKind kind, string value) { }
		}
	}
}