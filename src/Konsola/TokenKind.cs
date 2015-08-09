//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	/// <summary>
	/// Represents the token kind.
	/// </summary>
	internal enum TokenKind
	{
		/// <summary>
		/// A one word token.
		/// </summary>
		Word,

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