using System;
using System.Diagnostics;

namespace Konsola.Parser
{
	[DebuggerDisplay("Kind={Kind}, Value={Value}")]
	public abstract class Token
	{
		public Token(TokenKind kind, string value)
		{
			Kind = kind;
			Value = value;
		}

		public TokenKind Kind { get; private set; }

		public string Value { get; private set; }
	}

	public class CommandToken : Token
	{
		public CommandToken(string value)
			: base(TokenKind.Command, value)
		{
		}
	}

	public class OptionToken : Token
	{
		public OptionToken(string value)
			: base(TokenKind.Option, value)
		{
		}
	}

	public class DataToken : Token
	{
		public DataToken(string value)
			: base(TokenKind.Data, value)
		{
		}
	}

	public class SwitchToken : Token
	{
		public SwitchToken(string value)
			: base(TokenKind.Switch, value)
		{
		}
	}
}