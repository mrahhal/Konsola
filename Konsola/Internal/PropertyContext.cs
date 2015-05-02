//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using System;
using System.Reflection;

namespace Konsola.Internal
{
	internal struct PropertyContext
	{
		private PropertyInfo        _pi;
		private KParameterAttribute _attribute;

		public PropertyContext(PropertyInfo pi, KParameterAttribute attribute)
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

		public KParameterAttribute Attribute { get { return _attribute; } }
	}
}