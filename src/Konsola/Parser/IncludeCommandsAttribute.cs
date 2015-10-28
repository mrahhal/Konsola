using System;
using System.Linq;

namespace Konsola.Parser
{
	/// <summary>
	/// Specifies all the commands that are to be included in the decorated context.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class IncludeCommandsAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IncludeCommandsAttribute"/> class.
		/// </summary>
		/// <param name="commands">The commands.</param>
		public IncludeCommandsAttribute(params Type[] commands)
		{
			Commands = commands;
		}

		/// <summary>
		/// Gets the commands.
		/// </summary>
		public Type[] Commands { get; private set; }
	}
}