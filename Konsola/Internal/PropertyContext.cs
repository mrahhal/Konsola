//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using Konsola.Attributes;

namespace Konsola.Internal
{
	internal struct PropertyContext
	{
		private PropertyInfo _pi;
		private ParameterAttribute _attribute;

		public PropertyContext(PropertyInfo pi, ParameterAttribute attribute)
		{
			_pi = pi;
			_attribute = attribute;
		}

		public bool IsEmpty
		{
			get
			{
				return Property == null && Attribute == null;
			}
		}

		public PropertyInfo Property { get { return _pi; } }

		public ParameterAttribute Attribute { get { return _attribute; } }
	}
}