﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Internal
{
	/// <summary>
	/// Represents the token kind.
	/// </summary>
	internal enum TokenKind
	{
		/// <summary>
		/// A command token.
		/// </summary>
		Command,

		/// <summary>
		/// A param-value token.
		/// </summary>
		Full,

		/// <summary>
		/// A switch param token.
		/// </summary>
		Partial,
	}
}