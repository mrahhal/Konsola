using System.Collections.Generic;

namespace Konsola.Parser
{
	public class HelpContext
	{
		public ContextOptionsAttribute Options { get; set; }
		public IEnumerable<ParameterContext> Parameters { get; set; }
		public IEnumerable<CommandAttribute> NestedCommands { get; set; }
	}
}
