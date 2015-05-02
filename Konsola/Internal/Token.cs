﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Konsola.Internal
{
	/// <summary>
	/// Represents a token in a stream of command line arguments.
	/// </summary>
	[DebuggerDisplay("Kind={Kind}, Param={Param}, Value={Value}")]
	internal struct Token
	{
		private string _param;
		private string _value;
		private TokenKind _kind;

		public Token(string param, bool isCommand)
			: this(param, null)
		{
			if (isCommand)
			{
				_kind = TokenKind.Command;
			}
		}

		public Token(string param, string value)
		{
			_param = param;
			_value = value;
			_kind = _value == null ? TokenKind.Partial : TokenKind.Full;
		}

		public string Param
		{
			get { return _param; }
		}

		public string Value
		{
			get { return _value; }
		}

		public TokenKind Kind
		{
			get { return _kind; }
		}
	}
}