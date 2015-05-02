//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class KCommandAttribute : Attribute
	{
		public KCommandAttribute(string name)
		{
			Name = name;
			_Validate();
		}

		public string Name { get; private set; }

		private void _Validate()
		{
			if (string.IsNullOrWhiteSpace(Name) || Name.Any(c => KParameterAttribute.InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Command name is invalid.");
			}
		}
	}
}