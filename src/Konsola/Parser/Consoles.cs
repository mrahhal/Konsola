using System;

namespace Konsola.Parser
{
	public static class Consoles
	{
		/// <summary>
		/// Gets a console that silently consumes messages.
		/// </summary>
		public static IConsole Silent { get { return SilentConsole.Instance; } }
	}
}