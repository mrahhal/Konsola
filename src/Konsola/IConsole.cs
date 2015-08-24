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
		void Write(WriteKind kind, string value);
		void WriteLine(WriteKind kind, string value);
	}

	public static class ConsoleExtensions
	{
		public static void WriteLine(this IConsole @this)
		{
			@this.WriteLine(WriteKind.Normal, string.Empty);
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
	}
}