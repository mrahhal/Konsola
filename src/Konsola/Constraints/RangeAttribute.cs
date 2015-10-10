using System;

namespace Konsola.Constraints
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class RangeAttribute : ConstraintBaseAttribute
	{
		public RangeAttribute(int minValue, int maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}

		/// <summary>
		/// Gets or sets the minimum inclusive value.
		/// </summary>
		public int MinValue { get; private set; }

		/// <summary>
		/// Gets or sets the maximum value (exclusive by default).
		/// </summary>
		public int MaxValue { get; private set; }

		/// <summary>
		/// Gets or sets whether the max value is inclusive.
		/// </summary>
		public bool IsMaxInclusive { get; set; }

		public override string ErrorMessage
		{
			get
			{
				return "Not in correct range: " + ParameterName;
			}
		}

		public override bool Validate(object value)
		{
			var val = (int)value;
			if (val < MinValue)
			{
				return false;
			}
			if (!IsMaxInclusive && val >= MaxValue)
			{
				return false;
			}
			else if (IsMaxInclusive && val > MaxValue)
			{
				return false;
			}
			return true;
		}
	}
}