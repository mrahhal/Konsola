using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Specifies the default command to be invoked when the arguments do not explicitly specify command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class DefaultCommandAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCommandAttribute"/> class.
		/// </summary>
		/// <param name="command">The command's type.</param>
		public DefaultCommandAttribute(Type command)
		{
			Command = command;
		}

		/// <summary>
		/// Gets the command's type.
		/// </summary>
		public Type Command { get; private set; }
	}
}
