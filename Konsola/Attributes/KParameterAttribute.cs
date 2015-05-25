//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class KParameterAttribute : Attribute
	{
		internal const string InvalidCharacters = " -/\\";

		public KParameterAttribute(string parameters, bool isMandatory = false)
		{
			Parameters = parameters;
			IsMandatory = isMandatory;
			_Validate();
			_Initialize();
		}

		private void _Validate()
		{
			if (Parameters.Any((c) => InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Parameters contains invalid characters.");
			}
		}

		private void _Initialize()
		{
			InternalParameters = Parameters.Split(',');
		}

		public string Parameters { get; private set; }

		public bool IsMandatory { get; private set; }

		internal string[] InternalParameters { get; private set; }

		internal bool IsSet { get; set; }

		internal ParameterKind Kind { get; set; }

		public bool IsFlags { get; set; }

		public string[] ValidValues { get; set; }
	}
}