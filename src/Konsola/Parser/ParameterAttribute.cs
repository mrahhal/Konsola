using System;
using System.Linq;

namespace Konsola.Parser
{
	/// <summary>
	/// Decorates a property of a command class and makes it a binding target for arguments.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ParameterAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
		/// </summary>
		/// <param name="names">The names of the parameter.</param>
		public ParameterAttribute(string names)
		{
			Names = names;
			Position = 1000;
		}

		/// <summary>
		/// Gets the names of the parameter.
		/// </summary>
		public string Names { get; private set; }

		/// <summary>
		/// Gets or sets the parameter's description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this parameter is mandatory.
		/// </summary>
		public bool IsMandatory { get; set; }

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public int Position { get; set; }
	}
}