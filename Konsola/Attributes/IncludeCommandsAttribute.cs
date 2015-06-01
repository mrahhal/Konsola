//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class IncludeCommandsAttribute : Attribute
	{
		public IncludeCommandsAttribute(params Type[] commandTypes)
		{
			if (commandTypes == null || commandTypes.Length == 0)
			{
				throw new ContextException("Must contain command types.");
			}
			if (commandTypes.Any(ct => !typeof(CommandBase).IsAssignableFrom(ct)))
			{
				throw new ContextException("Commands must extend CommandBase.");
			}

			CommandTypes = commandTypes;
		}

		internal Type[] CommandTypes { get; set; }
	}
}