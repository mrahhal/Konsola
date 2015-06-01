﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Attributes
{
	/// <summary>
	/// Decorates a method in a command class.
	/// This becomes invokable when parsing and binding args finishes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class OnParsedAttribute : Attribute
	{
	}
}