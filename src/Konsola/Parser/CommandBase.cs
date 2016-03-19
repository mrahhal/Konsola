namespace Konsola.Parser
{
	/// <summary>
	/// Represents a command.
	/// </summary>
	public abstract class CommandBase
	{
		public CommandBase()
		{
		}

		internal ContextBase ContextBase { get; set; }

		internal CommandAttribute CommandAttribute { get; set; }

		internal IncludeCommandsAttribute IncludeCommandsAttribute { get; set; }

		public abstract void ExecuteCommand();
	}

	/// <summary>
	/// Represents a command associated to a context.
	/// </summary>
	public abstract class CommandBase<T> : CommandBase
		where T : ContextBase
	{
		public CommandBase()
		{
		}

		public T Context => (T)ContextBase;
	}
}
