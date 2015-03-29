//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola
{
	public enum ExceptionKind
	{
		MissingParameter,
		IncorrectParameter,
		MissingData,
		IncorrectData,

		FaultyData,
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

				case ExceptionKind.IncorrectData:
					Message = "Incorrect data: ";
					break;

				case ExceptionKind.MissingData:
					Message = "Missing data: ";
					break;

				case ExceptionKind.FaultyData:
					Message = "Faulty data: ";
					break;
			}
			Message += Name;
		}
	}
}