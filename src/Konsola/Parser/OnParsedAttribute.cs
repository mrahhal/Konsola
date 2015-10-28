using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Decorates a method in a command class.
	/// This becomes invokable after arguments are parsed and bound.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class OnParsedAttribute : Attribute
	{
	}
}