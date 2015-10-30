using System;

namespace Konsola.Parser
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

		/// <summary>
		/// A raw message.
		/// </summary>
		Message,
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

		public ContextException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	public class CommandLineException : Exception
	{
		public CommandLineException(string message)
			: base(message)
		{
		}

		public CommandLineException(CommandLineExceptionKind kind, string name)
		{
			Kind = kind;
			Name = name;
		}

		public CommandLineExceptionKind Kind { get; set; }

		public string Name { get; set; }
	}
}