//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Internal;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class IncludeCommandsAttribute : Attribute
	{

		public IncludeCommandsAttribute(params Type[] commands)
		{
			if (commands == null || commands.Length == 0)
			{
				throw new ContextException("Must contain command types.");
			}
			if (commands.Any(ct => !ct.IsCommandType()))
			{
				throw new ContextException("Commands must extend CommandBase.");
			}

			Commands = commands;
		}

		internal Type[] Commands { get; set; }
	}
}