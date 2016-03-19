using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Decorates a command class and specifies its binding options.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CommandAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandAttribute"/> class.
		/// </summary>
		/// <param name="name">The name of the command.</param>
		public CommandAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Gets or sets the name of the command which will be used when binding arguments.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the command's description.
		/// </summary>
		public string Description { get; set; }
	}
}
