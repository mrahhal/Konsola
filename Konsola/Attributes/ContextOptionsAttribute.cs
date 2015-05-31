//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ContextOptionsAttribute : Attribute
	{
		public ContextOptionsAttribute()
		{
		}

		public bool ExitOnException { get; set; }

		public bool InvokeMethods { get; set; }
	}
}