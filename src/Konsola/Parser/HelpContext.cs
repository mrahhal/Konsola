using System;
using System.Collections.Generic;
using System.Reflection;

namespace Konsola.Parser
{
	public class HelpContext
	{
		public ContextOptionsAttribute Options { get; set; }
		public IEnumerable<ParameterContext> Parameters { get; set; }
		public IEnumerable<CommandAttribute> NestedCommands { get; set; }
	}

	public class CommandHelpContext
	{
		public CommandAttribute Attribute { get; set; }
		public IEnumerable<ParameterContext> Parameters { get; set; }
		public IEnumerable<CommandAttribute> NestedCommands { get; set; }
	}

	public class ParameterContext
	{
		public ParameterKind Kind { get; set; }
		public PropertyInfo PropertyInfo { get; set; }
		public ParameterAttribute Attribute { get; set; }
		public string FullName { get; set; }
	}

	public enum ParameterKind
	{
		None,
		String,
		Int,
		Enum,
		Bool,
	}
}