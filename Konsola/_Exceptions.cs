//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public enum CommandLineExceptionKind
	{
		/// <summary>
		/// A mandatory parameter is missing.
		/// </summary>
		MissingParameter,

		/// <summary>
		/// Usage of the parameter is invalid.
		/// </summary>
		InvalidParameter,

		/// <summary>
		/// A value is missing.
		/// </summary>
		MissingValue,

		/// <summary>
		/// A value is invalid.
		/// </summary>
		InvalidValue,
	}

	[Serializable]
	public class ContextException : Exception
	{
		public ContextException()
		{
		}

		public ContextException(string message)
			: base(message)
		{
		}
	}

	[Serializable]
	public class CommandLineException : Exception
	{
		public CommandLineException(CommandLineExceptionKind kind, string name)
		{
			Kind = kind;
			Name = name;
			_Initialize();
		}

		public CommandLineExceptionKind Kind { get; set; }

		public string Name { get; set; }

		public new string Message { get; set; }

		private void _Initialize()
		{
			switch (Kind)
			{
				case CommandLineExceptionKind.MissingParameter:
					Message = "Missing parameter: ";
					break;

				case CommandLineExceptionKind.InvalidParameter:
					Message = "Invalid parameter: ";
					break;

				case CommandLineExceptionKind.MissingValue:
					Message = "Missing value: ";
					break;

				case CommandLineExceptionKind.InvalidValue:
					Message = "Invalid value: ";
					break;

			}
			Message += Name;
		}
	}
}