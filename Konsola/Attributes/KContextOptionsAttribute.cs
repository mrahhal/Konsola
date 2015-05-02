//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class KContextOptionsAttribute : Attribute
	{
		public KContextOptionsAttribute()
		{
		}

		public KContextOptionsAttribute(bool exitOnException)
		{
			ExitOnException = exitOnException;
		}

		public bool ExitOnException { get; private set; }
	}
}