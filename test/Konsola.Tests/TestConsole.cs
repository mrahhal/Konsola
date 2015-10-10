using System;
using System.Text;

namespace Konsola.Tests
{
	public class TestConsole : IConsole
	{
		private StringBuilder _sb = new StringBuilder();

		public string Text { get { return _sb.ToString(); } }

		public int WindowWidth
		{
			get { return -1; }
		}

		public int CursorLeft
		{
			get { return -1; }
		}

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