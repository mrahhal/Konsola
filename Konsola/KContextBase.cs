//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public abstract class KContextBase
	{
		public KContextBase()
		{
		}

		public KContextBase InnerContext { get; internal set; }
	}
}