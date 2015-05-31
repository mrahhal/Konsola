//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Internal;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ParameterAttribute : Attribute
	{
		internal const string InvalidCharacters = " -/\\";

		public ParameterAttribute(string parameters)
		{
			Parameters = parameters;
			_Validate();
			_Initialize();
		}

		public bool IsMandatory { get; set; }

		internal string Parameters { get; private set; }

		internal string[] InternalParameters { get; private set; }

		internal bool IsSet { get; set; }

		internal ParameterKind Kind { get; set; }

		internal bool IsFlags { get; set; }

		internal string[] ValidValues { get; set; }

		private void _Validate()
		{
			if (Parameters.Any((c) => InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Parameters contain invalid characters.");
			}
		}

		private void _Initialize()
		{
			InternalParameters = Parameters.Split(',');
		}
	}
}