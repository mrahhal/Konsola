using Konsola.Parser;
using System;

namespace Konsola.Parser
{
	public interface IErrorFormatter
	{
		void Format(CommandLineException ex, IConsole console);
	}
}