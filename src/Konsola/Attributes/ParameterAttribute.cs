//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Internal;

namespace Konsola.Attributes
{
	/// <summary>
	/// Decorates a property of a command class and makes it a binding target for args.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ParameterAttribute : Attribute
	{
		internal static readonly char[] InvalidCharacters = " /\\".ToCharArray();
		private ParameterKind _kind;

		public ParameterAttribute(string parameters)
		{
			Parameters = parameters;
			_Validate();
		}

		public string Description { get; set; }

		/// <summary>
		/// Gets or sets whether the decorated property is mandatory.
		/// </summary>
		public bool IsMandatory { get; set; }

		/// <summary>
		/// Gets or sets the parameter's position.
		/// </summary>
		public int Position { get; set; }

		#region Internal

		internal string Parameters { get; private set; }

		internal string[] InternalParameters { get; private set; }

		internal bool IsSet { get; set; }

		internal ParameterKind Kind
		{
			get { return _kind; }
			set
			{
				_kind = value;
				if (_kind == ParameterKind.Switch && IsMandatory)
				{
					throw new ContextException("Switch parameters don't make sense being mandatory.");
				}
			}
		}

		internal bool IsFlags { get; set; }

		internal string[] ValidValues { get; set; }

		#endregion

		private void _Validate()
		{
			InternalParameters = Parameters.Split(',');

			if (InternalParameters.Any((p) => p.StartsWith("-") || p.EndsWith("-"))
				|| Parameters.IndexOfAny(InvalidCharacters) != -1)
			{
				throw new ContextException("Parameters contain invalid characters.");
			}
		}
	}
}