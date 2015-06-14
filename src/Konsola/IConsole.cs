//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konsola
{
	public interface IConsole
	{
		/// <summary>
		/// Gets the console window's width, or -1 if not defined.
		/// </summary>
		int WindowWidth { get; }

		/// <summary>
		/// Gets the column position of the cursor, or -1 if not defined.
		/// </summary>
		int CursorLeft { get; }

		void Write(WriteKind kind, string value);
		void WriteLine(WriteKind kind, string value);
	}

	public static class ConsoleExtensions
	{
		public static void WriteLine(this IConsole @this)
		{
			@this.WriteLine(WriteKind.Normal, "");
		}

		public static void Write(this IConsole @this, string value)
		{
			@this.Write(WriteKind.Normal, value);
		}

		public static void Write(this IConsole @this, string format, params object[] args)
		{
			@this.Write(WriteKind.Normal, string.Format(format, args));
		}

		public static void WriteLine(this IConsole @this, string value)
		{
			@this.WriteLine(WriteKind.Normal, value);
		}

		public static void WriteLine(this IConsole @this, string format, params object[] args)
		{
			@this.WriteLine(WriteKind.Normal, string.Format(format, args));
		}

		public static void WriteJustified(this IConsole @this, int padding, string format, params object[] args)
		{
			@this.WriteJustified(padding, string.Format(format, args));
		}

		public static void WriteJustified(this IConsole @this, int padding, string value)
		{
			@this.WriteJustified(WriteKind.Normal, padding, value);
		}

		public static void WriteJustified(this IConsole @this, WriteKind kind, int padding, string format, params object[] args)
		{
			@this.WriteJustified(kind, padding, string.Format(format, args));
		}

		public static void WriteJustified(this IConsole @this, WriteKind kind, int padding, string value)
		{
			var width = @this.WindowWidth;
			width = width == -1 ? int.MaxValue : width;
			var maxWidth = width;
			if (maxWidth > padding)
			{
				maxWidth = maxWidth - padding - 1;
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

				var leftPadding = padding + content.Length - @this.CursorLeft;
				@this.WriteLine(kind, (leftPadding > 0) ? content.PadLeft(leftPadding) : content);
				value = value.Substring(content.Length);
			}
		}
	}
}