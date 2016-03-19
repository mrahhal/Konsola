namespace Konsola.Parser
{
	public enum TokenKind
	{
		Command,
		Option,
		Data,
		Switch,
	}

	public enum RawTokenKind
	{
		Raw,
		Option,
		Switch,
	}
}
