﻿using System;

namespace Konsola.Parser
{
	public class DefaultErrorFormatter : IErrorFormatter
	{
		public void Format(CommandLineException ex, IConsole console)
		{
			console.WriteLine(WriteKind.Error, GenerateMessage(ex));
		}

		private string GenerateMessage(CommandLineException ex)
		{
			var message = string.Empty;
			switch (ex.Kind)
			{
				case CommandLineExceptionKind.InvalidCommand:
					message = "Invalid command: ";
					break;

				case CommandLineExceptionKind.NoCommand:
					message = "No command has been specified";
					break;

				case CommandLineExceptionKind.MissingParameter:
					message = "Missing parameter: ";
					break;

				case CommandLineExceptionKind.InvalidParameter:
					message = "Invalid parameter: ";
					break;

				case CommandLineExceptionKind.MissingValue:
					message = "Missing value: ";
					break;

				case CommandLineExceptionKind.InvalidPositionalParameters:
					return "Positional parameters should come at the end";

				case CommandLineExceptionKind.InvalidValue:
					message = "Invalid value: ";
					break;

				case CommandLineExceptionKind.Invalid:
					return "Invalid arguments";

				case CommandLineExceptionKind.Message:
					return ex.Message;
			}
			message += ex.Name;

			return message;
		}
	}
}