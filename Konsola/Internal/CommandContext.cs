//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using System;

namespace Konsola.Internal
{
	internal struct CommandContext
	{
		private Type              _type;
		private KCommandAttribute _attribute;

		public CommandContext(Type type, KCommandAttribute attribute)
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

		public KCommandAttribute Attribute { get { return _attribute; } }
	}
}