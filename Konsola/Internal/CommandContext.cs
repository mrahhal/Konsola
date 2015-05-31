//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using Konsola.Attributes;

namespace Konsola.Internal
{
	internal struct CommandContext
	{
		private Type _type;
		private CommandAttribute _attribute;

		public CommandContext(Type type, CommandAttribute attribute)
		{
			_type = type;
			_attribute = attribute;
		}

		public bool IsEmpty
		{
			get
			{
				return Type == null && Attribute == null;
			}
		}

		public Type Type { get { return _type; } }

		public CommandAttribute Attribute { get { return _attribute; } }
	}
}