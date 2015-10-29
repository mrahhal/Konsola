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
		/// Gets or sets the program's description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to handle empty program invocations as help.
		/// </summary>
		public bool HandleEmptyInvocationAsHelp { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to invoke the special methods after parsing.
		/// </summary>
		public bool InvokeMethods { get; set; }
	}
}