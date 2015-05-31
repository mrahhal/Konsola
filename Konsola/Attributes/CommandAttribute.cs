//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CommandAttribute : Attribute
	{
		public CommandAttribute(string name)
		{
			Name = name;
			_Validate();
		}

		public string Name { get; private set; }

		private void _Validate()
		{
			if (string.IsNullOrWhiteSpace(Name) || Name.Any(c => ParameterAttribute.InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Command name is invalid.");
			}
		}
	}
}