using System;

namespace Konsola
{
	public enum CommandLineExceptionKind
	{
		/// <summary>
		/// A command is invalid.
		/// </summary>
		InvalidCommand,

		/// <summary>
		/// No command has been specified.
		/// </summary>
		NoCommand,

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

		/// <summary>
		/// Positional parameters are invalid.
		/// </summary>
		InvalidPositionalParameters,

		/// <summary>
		/// A constraint has been violated.
		/// </summary>
		Constraint,

		/// <summary>
		/// A general exception kind.
		/// </summary>
		Invalid,
	}

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

	public class CommandLineException : Exception
	{
		public CommandLineException(CommandLineExceptionKind kind, string name)
			: this(kind, name, null)
		{
		}

		public CommandLineException(CommandLineExceptionKind kind, string name, string message)
		{
			Kind = kind;
			Name = name;
			Message = message;
			_Initialize();
		}

		public CommandLineExceptionKind Kind { get; set; }

		public string Name { get; set; }

		public new string Message { get; set; }

		private void _Initialize()
		{
			if (Message != null)
			{
				return;
			}

			switch (Kind)
			{
				case CommandLineExceptionKind.InvalidCommand:
					Message = "Invalid command: ";
					break;

				case CommandLineExceptionKind.NoCommand:
					Message = "No command has been specified";
					break;

				case CommandLineExceptionKind.MissingParameter:
					Message = "Missing parameter: ";
					break;

				case CommandLineExceptionKind.InvalidParameter:
					Message = "Invalid parameter: ";
					break;

				case CommandLineExceptionKind.MissingValue:
					Message = "Missing value: ";
					break;

				case CommandLineExceptionKind.InvalidPositionalParameters:
					Message = "Positional parameters should come at the end";
					return;

				case CommandLineExceptionKind.InvalidValue:
					Message = "Invalid value: ";
					break;

				case CommandLineExceptionKind.Invalid:
					Message = "Invalid arguments";
					return;
			}
			Message += Name;
		}
	}
}