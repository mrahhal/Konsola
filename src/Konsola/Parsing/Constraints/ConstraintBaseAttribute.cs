using System;

namespace Konsola.Parsing.Constraints
{
	/// <summary>
	/// Represents a constraint that can be applied to a parameter.
	/// </summary>
	public abstract class ConstraintBaseAttribute : Attribute
	{
		public ConstraintBaseAttribute()
		{
		}

		/// <summary>
		/// Gets the parameter's name that this constraint is applied to.
		/// </summary>
		public string ParameterName { get; internal set; }

		/// <summary>
		/// Gets the error message to show when the constraint is violated.
		/// </summary>
		public virtual string ErrorMessage
		{
			get { return "Constraint error: " + ParameterName; }
		}

		/// <summary>
		/// Validates the value.
		/// </summary>
		/// <returns>true if value is accepted, false otherwise.</returns>
		public abstract bool Validate(object value);
	}
}