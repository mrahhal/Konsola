﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	/// <summary>
	/// Represents a context.
	/// </summary>
	public abstract class ContextBase
	{
		public ContextBase InnerContext { get; internal set; }
	}
}