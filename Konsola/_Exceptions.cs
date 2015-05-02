//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public enum ExceptionKind
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
	public class ParsingException : Exception
	{
		public ParsingException(ExceptionKind kind, string name)
		{
			Kind = kind;
			Name = name;
			_Initialize();
		}

		public ExceptionKind Kind { get; set; }

		public string Name { get; set; }

		public new string Message { get; set; }

		private void _Initialize()
		{
			switch (Kind)
			{
				case ExceptionKind.MissingParameter:
					Message = "Missing parameter: ";
					break;

				case ExceptionKind.InvalidParameter:
					Message = "Invalid parameter: ";
					break;

				case ExceptionKind.MissingValue:
					Message = "Missing value: ";
					break;

				case ExceptionKind.InvalidValue:
					Message = "Invalid value: ";
					break;

			}
			Message += Name;
		}
	}
}