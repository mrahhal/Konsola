namespace Konsola.Parser
{
	public class DataSource
	{
		public RawTokenKind Kind { get; set; }
		public string Identifier { get; set; }
		public string FullIdentifier { get; set; }
		public string Value { get; set; }
	}
}