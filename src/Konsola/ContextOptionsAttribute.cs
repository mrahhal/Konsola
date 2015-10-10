using System;

namespace Konsola
{
	/// <summary>
	/// Configures the options when parsing args and binding them to a context.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ContextOptionsAttribute : Attribute
	{
		public ContextOptionsAttribute()
		{
		}

		public string Description { get; set; }

#if NET40
		/// <summary>
		/// Gets or sets whether to print exception message and exit when
		/// a CommandLineException occures.
		/// </summary>
		public bool ExitOnException { get; set; }
#endif

		/// <summary>
		/// Gets or sets whether to invoke the special methods after parsing.
		/// </summary>
		public bool InvokeMethods { get; set; }
	}
}