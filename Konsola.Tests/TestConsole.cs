//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Tests
{
	public class TestConsole : IConsole
	{
		public void Write(string value)
		{
		}

		public void Write(string format, params object[] args)
		{
		}

		public void WriteLine()
		{
		}

		public void WriteLine(string value)
		{
		}

		public void WriteLine(string format, params object[] args)
		{
		}

		public void WriteWarning(string value)
		{
		}

		public void WriteWarning(string format, params object[] args)
		{
		}

		public void WriteWarningLine(string value)
		{
		}

		public void WriteWarningLine(string format, params object[] args)
		{
		}

		public void WriteError(string value)
		{
		}

		public void WriteError(string format, params object[] args)
		{
		}

		public void WriteErrorLine(string value)
		{
		}

		public void WriteErrorLine(string format, params object[] args)
		{
		}

		public void WriteColor(ConsoleColor color, string value)
		{
		}

		public void WriteColor(ConsoleColor color, string format, params object[] args)
		{
		}

		public void WriteColorLine(ConsoleColor color, string value)
		{
		}

		public void WriteColorLine(ConsoleColor color, string format, params object[] args)
		{
		}

		public void WriteJustified(int startIndex, string value)
		{
		}

		public void WriteJustified(int startIndex, string value, int maxWidth)
		{
		}

		public void WriteJustified(ConsoleColor color, int startIndex, string value, int maxWidth)
		{
		}
	}
}