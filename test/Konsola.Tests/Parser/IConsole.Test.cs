using System;
using System.Text;

namespace Konsola.Parser.Tests
{
	public class TestConsole : IConsole
	{
		private StringBuilder _sb = new StringBuilder();

		public string Text { get { return _sb.ToString(); } }

		public void Write(WriteKind kind, string value)
		{
			_sb.Append(value);
		}
	}
}