﻿using System;

namespace Konsola
{
	internal static class Boxes
	{
		static Boxes()
		{
			False = (object)false;
			True = (object)true;
		}

		public static object False { get; private set; }
		public static object True { get; private set; }
	}
}