using System.Linq;
using System.Collections.Generic;

namespace Konsola.Parser
{
	public class DefaultHelpFormatter : IHelpFormatter
	{
		public void Format(HelpContext context, IConsole console)
		{
			if (!string.IsNullOrWhiteSpace(context.Options.Description))
			{
				console.WriteLine(context.Options.Description);
				console.WriteLine();
			}
			if (context.NestedCommands.Any())
			{
				FormatCommands(context.NestedCommands, console);
				console.WriteLine();
			}
			if (context.Parameters.Any())
			{
				FormatParameters(context.Parameters, console);
			}
		}

		public void FormatForCommand(CommandHelpContext context, IConsole console)
		{
			if (!string.IsNullOrWhiteSpace(context.Attribute.Description))
			{
				console.WriteLine(context.Attribute.Description);
				console.WriteLine();
			}
			if (context.NestedCommands.Any())
			{
				FormatCommands(context.NestedCommands, console);
				console.WriteLine();
			}
			if (context.Parameters.Any())
			{
				FormatParameters(context.Parameters, console);
			}
		}

		private void FormatParameters(IEnumerable<ParameterContext> parameters, IConsole console)
		{
			console.WriteLine(WriteKind.Info, "Parameters:");
			foreach (var parameter in parameters)
			{
				console.Write("* " + parameter.FullName);
				if (!string.IsNullOrWhiteSpace(parameter.Attribute.Description))
				{
					console.WriteLine(" - " + parameter.Attribute.Description);
				}
				else
				{
					console.WriteLine();
				}
			}
		}

		private void FormatCommands(IEnumerable<CommandAttribute> commands, IConsole console)
		{
			console.WriteLine(WriteKind.Info, "Commands:");
			foreach (var command in commands)
			{
				console.Write("* " + command.Name);
				if (!string.IsNullOrWhiteSpace(command.Description))
				{
					console.WriteLine(" - " + command.Description);
				}
				else
				{
					console.WriteLine();
				}
			}
		}
	}
}