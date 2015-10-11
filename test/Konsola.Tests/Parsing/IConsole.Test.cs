using System;
using System.Text;

namespace Konsola.Parsing.Tests
{
	public class TestConsole : IConsole
	{
		private StringBuilder _sb = new StringBuilder();

		public string Text { get { return _sb.ToString(); } }

		public void Write(WriteKind kind, string value)
		{
			_sb.Append(value);
		}

		public void WriteLine(WriteKind kind, string value)
		{
			Write(kind, value + Environment.NewLine);
		}
	}
}