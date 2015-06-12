//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
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

		public int WindowWidth { get { return -1; } }
		public int CursorLeft { get { return -1; } }

		public void Write(WriteKind kind, string value) { }
		public void WriteLine(WriteKind kind, string value) { }
	}
}