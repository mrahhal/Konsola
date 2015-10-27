using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konsola.Parsing
{
	public interface IConsole
	{
		void Write(WriteKind kind, string value);
	}

	public static class ConsoleExtensions
	{
		public static void WriteLine(this IConsole @this, WriteKind kind, string value)
		{
			@this.Write(kind, value + Environment.NewLine);
		}

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
	}
}