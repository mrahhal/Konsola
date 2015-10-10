using System;
using System.Reflection;
using Konsola.Constraints;

namespace Konsola
{
	internal class ParameterContext
	{
		private PropertyInfo _pi;
		private ParameterAttribute _parameterAttribute;
		private ConstraintBaseAttribute[] _constraints;

		public ParameterContext(PropertyInfo pi)
		{
			_pi = pi;
			_Initialize();
		}

		private void _Initialize()
		{
			_parameterAttribute = _pi.GetCustomAttribute<ParameterAttribute>();
			if (_parameterAttribute == null)
				return;

			_constraints = _pi.GetCustomAttributes<ConstraintBaseAttribute>();
			foreach (var constraint in _constraints)
			{
				constraint.ParameterName = _parameterAttribute.InternalParameters[0];
			}
		}

		public PropertyInfo Property { get { return _pi; } }

		public ParameterAttribute ParameterAttribute { get { return _parameterAttribute; } }

		public ConstraintBaseAttribute[] ConstraintAttributes { get { return _constraints; } }
	}
}