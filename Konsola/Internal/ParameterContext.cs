//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using Konsola.Attributes;

namespace Konsola.Internal
{
	internal class ParameterContext
	{
		private PropertyInfo _pi;
		private ParameterAttribute _parameterAttribute;

		public ParameterContext(PropertyInfo pi)
		{
			_pi = pi;
			_Initialize();
		}

		private void _Initialize()
		{
			_parameterAttribute = _pi.GetCustomAttribute<ParameterAttribute>();
		}

		public PropertyInfo Property { get { return _pi; } }

		public ParameterAttribute ParameterAttribute { get { return _parameterAttribute; } }

	}
}
