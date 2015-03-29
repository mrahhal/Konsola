//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Konsola.Internal
{
	[DebuggerDisplay("Kind={Kind}, Value={Value}")]
	internal class Token
	{
		private string _value;

		public Token(string value, TokenKind kind, Token previous)
		{
			Debug.Assert(!value.StartsWith("-") && !value.StartsWith("--"));
			_value = value;
			Previous = previous;
			Kind = kind;
		}

		public string Value { get { return _value; } }

		public Token Previous { get; set; }

		public Token Next { get; set; }

		public TokenKind Kind { get; set; }
	}
}