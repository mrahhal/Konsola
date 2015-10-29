using System;

namespace Konsola.Parser
{
	public class DefaultTokenizer : Tokenizer
	{
		public override string OptionPre
		{
			get
			{
				return "-";
			}
		}

		public override string SwitchPre
		{
			get
			{
				return "--";
			}
		}
	}
}