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
		}

		public string Parameters { get; set; }

		public ParameterKind Kind { get; set; }

		public bool IsMandantory { get; set; }
	}
}