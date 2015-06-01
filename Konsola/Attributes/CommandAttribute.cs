//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Attributes
{
	/// <summary>
	/// Decorates a command class and specifies its binding options.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CommandAttribute : Attribute
	{
		public CommandAttribute(string name)
		{
			Name = name;
			_Validate();
		}

		/// <summary>
		/// Gets or sets the name of the command which will also be used when binding args.
		/// </summary>
		internal string Name { get; private set; }

		private void _Validate()
		{
			if (string.IsNullOrWhiteSpace(Name) || Name.Any(c => ParameterAttribute.InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Command name is invalid.");
			}
		}
	}
}