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
		private ConsoleColor _color;

		public Console()
		{
			_color = SConsole.ForegroundColor;
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

		public void Write(string value)
		{
			WriteColor(_color, value);
		}

		public void Write(string format, params object[] args)
		{
			WriteColor(_color, format, args);
		}

		public void WriteLine()
		{
			SConsole.WriteLine();
		}

		public void WriteLine(string value)
		{
			WriteColorLine(_color, value);
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteColorLine(_color, format, args);
		}

		public void WriteWarning(string value)
		{
			WriteColor(ConsoleColor.Yellow, value);
		}

		public void WriteWarning(string format, params object[] args)
		{
			WriteColor(ConsoleColor.Yellow, format, args);
		}

		public void WriteWarningLine(string value)
		{
			WriteColorLine(ConsoleColor.Yellow, value);
		}

		public void WriteWarningLine(string format, params object[] args)
		{
			WriteColorLine(ConsoleColor.Yellow, format, args);
		}

		public void WriteError(string value)
		{
			WriteColor(ConsoleColor.Red, value);
		}

		public void WriteError(string format, params object[] args)
		{
			WriteColor(ConsoleColor.Red, format, args);
		}

		public void WriteErrorLine(string value)
		{
			WriteColorLine(ConsoleColor.Red, value);
		}

		public void WriteErrorLine(string format, params object[] args)
		{
			WriteColorLine(ConsoleColor.Red, format, args);
		}

		public void WriteColor(ConsoleColor color, string value)
		{
			_SafeWriteColor(color, false, value);
		}

		public void WriteColor(ConsoleColor color, string format, params object[] args)
		{
			_SafeWriteColor(color, false, format, args);
		}

		public void WriteColorLine(ConsoleColor color, string value)
		{
			_SafeWriteColor(color, true, value);
		}

		public void WriteColorLine(ConsoleColor color, string format, params object[] args)
		{
			_SafeWriteColor(color, true, format, args);
		}

		public void WriteJustified(int startIndex, string value)
		{
			WriteJustified(startIndex, value, WindowWidth);
		}

		public void WriteJustified(int startIndex, string value, int maxWidth)
		{
			WriteJustified(_color, startIndex, value, maxWidth);
		}

		public void WriteJustified(ConsoleColor color, int startIndex, string value, int maxWidth)
		{
			if (maxWidth > startIndex)
			{
				maxWidth = maxWidth - startIndex - 1;
			}

			while (value.Length > 0)
			{
				value = value.TrimStart();
				int length = Math.Min(value.Length, maxWidth);

				var content = default(string);

				int newLineIndex = value.IndexOf(Environment.NewLine, 0, length, StringComparison.OrdinalIgnoreCase);
				if (newLineIndex > -1)
				{
					content = value.Substring(0, newLineIndex);
				}
				else
				{
					content = value.Substring(0, length);
				}

				var leftPadding = startIndex + content.Length - CursorLeft;
				WriteColorLine(color, (leftPadding > 0) ? content.PadLeft(leftPadding) : content);
				value = value.Substring(content.Length);
			}
		}

		private void _SafeWriteColor(ConsoleColor color, bool newLine, string value)
		{
			var prevColor = _color;
			_color = color;
			try
			{
				if (!newLine)
				{
					SConsole.Write(value);
				}
				else
				{
					SConsole.WriteLine(value);
				}
			}
			finally
			{
				_color = prevColor;
			}
		}

		private void _SafeWriteColor(ConsoleColor color, bool newLine, string format, params object[] args)
		{
			var prevColor = _color;
			_color = color;
			try
			{
				if (!newLine)
				{
					SConsole.Write(format, args);
				}
				else
				{
					SConsole.WriteLine(format, args);
				}
			}
			finally
			{
				_color = prevColor;
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

			public void Write(string value) { }
			public void Write(string format, params object[] args) { }
			public void WriteLine() { }
			public void WriteLine(string value) { }
			public void WriteLine(string format, params object[] args) { }
			public void WriteWarning(string value) { }
			public void WriteWarning(string format, params object[] args) { }
			public void WriteWarningLine(string value) { }
			public void WriteWarningLine(string format, params object[] args) { }
			public void WriteError(string value) { }
			public void WriteError(string format, params object[] args) { }
			public void WriteErrorLine(string value) { }
			public void WriteErrorLine(string format, params object[] args) { }
			public void WriteColor(ConsoleColor color, string value) { }
			public void WriteColor(ConsoleColor color, string format, params object[] args) { }
			public void WriteColorLine(ConsoleColor color, string value) { }
			public void WriteColorLine(ConsoleColor color, string format, params object[] args) { }
			public void WriteJustified(int startIndex, string value) { }
			public void WriteJustified(int startIndex, string value, int maxWidth) { }
			public void WriteJustified(ConsoleColor color, int startIndex, string value, int maxWidth) { }
		}
	}
}