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
		void Write(string value);
		void Write(string format, params object[] args);
		void WriteLine();
		void WriteLine(string value);
		void WriteLine(string format, params object[] args);

		void WriteWarning(string value);
		void WriteWarning(string format, params object[] args);
		void WriteWarningLine(string value);
		void WriteWarningLine(string format, params object[] args);

		void WriteError(string value);
		void WriteError(string format, params object[] args);
		void WriteErrorLine(string value);
		void WriteErrorLine(string format, params object[] args);

		void WriteColor(ConsoleColor color, string value);
		void WriteColor(ConsoleColor color, string format, params object[] args);
		void WriteColorLine(ConsoleColor color, string value);
		void WriteColorLine(ConsoleColor color, string format, params object[] args);

		void WriteJustified(int startIndex, string value);
		void WriteJustified(int startIndex, string value, int maxWidth);
	}
}