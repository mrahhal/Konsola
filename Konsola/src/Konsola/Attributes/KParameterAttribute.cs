//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;

namespace Konsola.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class KParameterAttribute : Attribute
	{
		public KParameterAttribute(string parameters, ParameterKind kind, bool isMandantory = false)
		{
			Parameters = parameters;
			Kind = kind;
			IsMandantory = isMandantory;
			_Validate();
			_Initialize();
		}

		private void _Validate()
		{
			if (Parameters.Contains(" ") || Parameters.Contains("-"))
			{
				throw new ContextException("Parameters contains invalid characters.");
			}
		}

		private void _Initialize()
		{
			InternalParameters = Parameters.Split(';');
		}

		public string Parameters { get; set; }

		internal string[] InternalParameters { get; set; }

		public ParameterKind Kind { get; set; }

		public bool IsMandantory { get; set; }
	}
}