using System;

namespace Konsola.Parsing
{
	internal class CommandContext
	{
		private Type _type;
		private CommandAttribute _attribute;

		public CommandContext(Type type)
		{
			_type = type;
			_Initialize();
		}

		private void _Initialize()
		{
			_attribute = _type.GetCustomAttribute<CommandAttribute>();
		}

		public Type Type { get { return _type; } }

		public CommandAttribute Attribute { get { return _attribute; } }
	}
}