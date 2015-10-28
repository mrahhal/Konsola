using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Represents the context of parsed arguments.
	/// </summary>
	public abstract class ContextBase
	{
		/// <summary>
		/// Gets the command.
		/// </summary>
		public CommandBase Command { get; internal set; }

		/// <summary>
		/// Gets or sets the program's description.
		/// </summary>
		public virtual string Description { get; set;  }
	}
}