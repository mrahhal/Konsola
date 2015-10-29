using System;

namespace Konsola.Parser
{
	public abstract class Tokenizer
	{
		private Func<string, RawTokenKind> _processor;

		public Token[] Process(string[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			var tokens = new Token[args.Length];
			var madeFirstContact = false;
			for (int i = 0; i < args.Length; i++)
			{
				var value = args[i];
				var rawKind = ProcessCore(value);

				if (rawKind == RawTokenKind.Option || rawKind == RawTokenKind.Switch)
				{
					madeFirstContact = true;
				}

				if (rawKind == RawTokenKind.Raw && madeFirstContact)
				{
					tokens[i] = new DataToken(value);
				}
				else if (rawKind == RawTokenKind.Raw)
				{
					tokens[i] = new CommandToken(value);
				}
				else if (rawKind == RawTokenKind.Option)
				{
					tokens[i] = new OptionToken(value);
				}
				else if (rawKind == RawTokenKind.Switch)
				{
					tokens[i] = new SwitchToken(value);
				}
			}

			return tokens;
		}

		public virtual RawTokenKind ProcessCore(string value)
		{
			if (_processor == null)
			{
				if (OptionPre.Length >= SwitchPre.Length)
				{
					_processor = (v) =>
						{
							if (v.StartsWith(OptionPre))
							{
								return RawTokenKind.Switch;
							}
							else if (v.StartsWith(SwitchPre))
							{
								return RawTokenKind.Option;
							}
							else
							{
								return RawTokenKind.Raw;
							}
						};
				}
				else
				{
					_processor = (v) =>
					{
						if (v.StartsWith(SwitchPre))
						{
							return RawTokenKind.Switch;
						}
						else if (v.StartsWith(OptionPre))
						{
							return RawTokenKind.Option;
						}
						else
						{
							return RawTokenKind.Raw;
						}
					};
				}
			}

			return _processor(value);
		}

		public string ExtractIdentifier(TokenKind kind, string value)
		{
			switch (kind)
			{
				case TokenKind.Option:
					return value.Substring(OptionPre.Length);
				case TokenKind.Switch:
					return value.Substring(SwitchPre.Length);
				default:
					return value;
			}
		}

		public abstract string OptionPre { get; }

		public abstract string SwitchPre { get; }
	}
}