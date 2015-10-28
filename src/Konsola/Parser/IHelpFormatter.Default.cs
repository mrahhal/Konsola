using System.Linq;
using System.Collections.Generic;

namespace Konsola.Parser
{
	public class DefaultHelpFormatter : IHelpFormatter
	{
		public void Format(HelpContext context, IConsole console)
		{
			console.WriteLine(context.Options.Description);
			console.WriteLine();
			FormatCommands(context.NestedCommands, console);
			console.WriteLine();
			FormatParameters(context.Parameters, console);
		}

		public void FormatForCommand(CommandHelpContext context, IConsole console)
		{
			console.WriteLine(context.Attribute.Description);
			console.WriteLine();
			FormatCommands(context.NestedCommands, console);
			console.WriteLine();
			FormatParameters(context.Parameters, console);
		}

		private void FormatParameters(IEnumerable<ParameterContext> parameters, IConsole console)
		{
			if (!parameters.Any())
			{
				return;
			}

			console.WriteLine(WriteKind.Info, "Parameters:");
			foreach (var parameter in parameters)
			{
				console.WriteLine("* " + parameter.FullName + " - " + parameter.Attribute.Description);
			}
		}

		private void FormatCommands(IEnumerable<CommandAttribute> commands, IConsole console)
		{
			if (!commands.Any())
			{
				return;
			}

			console.WriteLine(WriteKind.Info, "Commands:");
			foreach (var command in commands)
			{
				console.WriteLine("* " + command.Name + " - " + command.Description);
			}
		}
	}
}