//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using Konsola.Attributes;

namespace Konsola.Internal
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