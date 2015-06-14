﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Konsola
{
	public static class KUtils
	{
		public static IEnumerable<string> SplitCommandLineArgs(string args)
		{
			var inQuotes = false;

			return _Split(args, c =>
			{
				if (c == '\"')
					inQuotes = !inQuotes;

				return !inQuotes && c == ' ';
			})
			.Select(arg => _TrimMatchingQuotes(arg.Trim(), '\"'))
			.Where(arg => !string.IsNullOrEmpty(arg));
		}

		private static IEnumerable<string> _Split(string str, Func<char, bool> controller)
		{
			var nextPiece = default(int);

			for (int c = 0; c < str.Length; c++)
			{
				if (controller(str[c]))
				{
					yield return str.Substring(nextPiece, c - nextPiece);
					nextPiece = c + 1;
				}
			}

			yield return str.Substring(nextPiece);
		}

		private static string _TrimMatchingQuotes(string input, char quote)
		{
			if ((input.Length >= 2) &&
				(input[0] == quote) && (input[input.Length - 1] == quote))
				return input.Substring(1, input.Length - 2);

			return input;
		}
	}
}