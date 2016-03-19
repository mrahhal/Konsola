using System.Collections.Generic;

namespace Konsola.Parser
{
	public class CommandHelpContext
	{
		public CommandAttribute Attribute { get; set; }
		public IEnumerable<ParameterContext> Parameters { get; set; }
		public IEnumerable<CommandAttribute> NestedCommands { get; set; }
	}
}
