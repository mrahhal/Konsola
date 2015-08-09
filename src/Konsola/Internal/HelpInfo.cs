//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Internal
{
	internal class HelpInfo
	{
		public string ProgramDescription { get; set; }

		public CommandAttribute[] Commands { get; set; }

		public ParameterAttribute[] Parameters { get; set; }

		public HelpInfo(CommandBase command, ParameterContext[] pcs = null)
		{
			if (pcs == null)
			{
				pcs = command.GetType().GetPropertyContexts().ToArray();
			}
			var isDefault = _IsDefaultCommand(command);
			if (isDefault)
			{
				ProgramDescription = command.ContextBase.Options.Description;
				if (command.ContextBase.IncludeCommandsAttribute != null)
				{
					Commands = command.ContextBase.IncludeCommandsAttribute.Commands
						.Select(c => c.GetCustomAttribute<CommandAttribute>())
						.ToArray();
				}
			} else
			{
				if (command.IncludeCommandsAttribute != null)
				{
					Commands = command.IncludeCommandsAttribute.Commands
						.Select(c => c.GetCustomAttribute<CommandAttribute>())
						.ToArray();
				}
			}
			Parameters = pcs.Select(pc => pc.ParameterAttribute).ToArray();
		}

		private bool _IsDefaultCommand(CommandBase command)
		{
			var defaultAttribute = command.ContextBase.DefaultCommandAttribute;
			if (defaultAttribute == null)
				return false;
			return defaultAttribute.DefaultCommand == command.GetType();
		}
	}
}