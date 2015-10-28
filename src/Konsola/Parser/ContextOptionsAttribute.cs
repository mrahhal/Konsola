using System;

namespace Konsola.Parser
{
	/// <summary>
	/// Configures the options when parsing args and binding them to a context.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ContextOptionsAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether to invoke the special methods after parsing.
		/// </summary>
		/// <value>
		///   <c>true</c> to invoke methods; otherwise, <c>false</c>.
		/// </value>
		public bool InvokeMethods { get; set; }
	}
}