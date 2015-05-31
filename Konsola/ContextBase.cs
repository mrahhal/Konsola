//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public abstract class ContextBase
	{
		public ContextBase InnerContext { get; internal set; }
	}

	public abstract class CommandBase : ContextBase
	{
	}
}