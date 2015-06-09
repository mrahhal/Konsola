//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Attributes
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

		/// <summary>
		/// Gets or sets whether to print exception message and exit when
		/// a CommandLineException occures.
		/// </summary>
		public bool ExitOnException { get; set; }

		/// <summary>
		/// Gets or sets whether to invoke the special methods after parsing.
		/// </summary>
		public bool InvokeMethods { get; set; }
	}
}