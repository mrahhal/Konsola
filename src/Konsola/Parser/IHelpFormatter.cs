namespace Konsola.Parser
{
	public interface IHelpFormatter
	{
		void Format(HelpContext context, IConsole console);

		void FormatForCommand(CommandHelpContext context, IConsole console);
	}
}