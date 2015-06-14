﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public static class Consoles
	{
		/// <summary>
		/// Gets a console that silently consumes messages.
		/// </summary>
		public static IConsole Silent { get { return SilentConsole.Instance; } }
	}
}