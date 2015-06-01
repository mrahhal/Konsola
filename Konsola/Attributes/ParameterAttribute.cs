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
		internal const string InvalidCharacters = " /\\";

		public ParameterAttribute(string parameters)
		{
			Parameters = parameters;
			_Validate();
		}

		public bool IsMandatory { get; set; }

		#region Internal

		internal string Parameters { get; private set; }

		internal string[] InternalParameters { get; private set; }

		internal bool IsSet { get; set; }

		internal ParameterKind Kind { get; set; }

		internal bool IsFlags { get; set; }

		internal string[] ValidValues { get; set; }

		#endregion

		private void _Validate()
		{
			InternalParameters = Parameters.Split(',');

			if (InternalParameters.Any((p) => p.StartsWith("-") || p.EndsWith("-"))
				|| Parameters.Any((c) => InvalidCharacters.Contains(c)))
			{
				throw new ContextException("Parameters contain invalid characters.");
			}
		}
	}
}