using System;

namespace Konsola.Parsing
{
	/// <summary>
	/// Represents a parsed context.
	/// </summary>
	public abstract class ContextBase
	{
		/// <summary>
		/// Gets the command.
		/// </summary>
		public CommandBase Command { get; internal set; }

		/// <summary>
		/// Gets the options.
		/// </summary>
		public ContextOptionsAttribute Options { get; internal set; }

		internal IncludeCommandsAttribute IncludeCommandsAttribute { get; set; }

		internal DefaultCommandAttribute DefaultCommandAttribute { get; set; }
	}
}