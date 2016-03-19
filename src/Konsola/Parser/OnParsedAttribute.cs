using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Decorates a method in a command class.
	/// This becomes invoked after arguments are parsed and the command is bound.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class OnParsedAttribute : Attribute
	{
	}
}
