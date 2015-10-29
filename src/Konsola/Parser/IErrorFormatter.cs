using Konsola.Parser;
using System;

namespace Konsola.Parser
{
	public interface IErrorFormatter
	{
		string Format(CommandLineException ex);
	}
}