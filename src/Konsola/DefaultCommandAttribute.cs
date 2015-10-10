using System;

namespace Konsola
{
	/// <summary>
	/// Specifies the default command to be invoked when the args do not specify
	/// a command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class DefaultCommandAttribute : Attribute
	{
		private Type _defaultCommand;

		public DefaultCommandAttribute(Type defaultCommand)
		{
			DefaultCommand = defaultCommand;
		}

		internal Type DefaultCommand
		{
			get { return _defaultCommand; }
			set
			{
				if (value == null)
				{
					throw new ContextException("Should not be null.");
				}
				if (!value.IsCommandType())
				{
					throw new ContextException("Commands must extend CommandBase.");
				}

				_defaultCommand = value;
			}
		}
	}
}
