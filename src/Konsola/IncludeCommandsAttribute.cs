﻿using System;
using System.Linq;

namespace Konsola
{
	/// <summary>
	/// Specifies all the commands that are associated to the decorated context.
	/// </summary>
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