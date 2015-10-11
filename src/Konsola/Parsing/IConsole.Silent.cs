using System;

namespace Konsola.Parsing
{
	internal class SilentConsole : IConsole
	{
		private static readonly SilentConsole s_singleton;

		static SilentConsole()
		{
			s_singleton = new SilentConsole();
		}

		private SilentConsole()
		{
		}

		public static IConsole Instance
		{
			get
			{
				return s_singleton;
			}
		}

		public void Write(WriteKind kind, string value) { }
		public void WriteLine(WriteKind kind, string value) { }
	}
}