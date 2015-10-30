using System.Diagnostics;

namespace Konsola.Parser
{
	[DebuggerDisplay("Kind={Kind}, Context={Context}")]
	public class ParsingResult<T>
	{
		public ParsingResultKind Kind { get; set; }
		public T Context { get; set; }
		public CommandLineException Exception { get; set; }
	}
}