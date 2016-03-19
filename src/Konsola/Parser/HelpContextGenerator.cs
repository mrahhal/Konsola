using System;
using System.Collections.Generic;
using System.Linq;
using Konsola.Metadata;

namespace Konsola.Parser
{
	public class HelpContextGenerator
	{
		private ParameterContextProvider _provider;

		public HelpContextGenerator(ParameterContextProvider provider)
		{
			_provider = provider;
		}

		/// <summary>
		/// Generates the <see cref="HelpContext"/> for the specified context type.
		/// </summary>
		/// <param name="type">This is expected to be a ContextBase.</param>
		/// <exception cref="InvalidOperationException">The type is not a ContextBase.</exception>
		public HelpContext Generate(Type type)
		{
			if (!typeof(ContextBase).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("type should be a ContextBase.");
			}

			var metadata = MetadataProviders.Current.GetFor(type);
			var options =
				metadata.Attributes.FirstOrDefaultOfRealType<ContextOptionsAttribute>() ??
				new ContextOptionsAttribute();
			var defaultCommand = metadata.Attributes.FirstOrDefaultOfRealType<DefaultCommandAttribute>();
			var nestedCommands = metadata.Attributes.FirstOrDefaultOfRealType<IncludeCommandsAttribute>();

			var parameters = default(IEnumerable<ParameterContext>);
			var commands = default(IEnumerable<CommandAttribute>);

			if (defaultCommand != null)
			{
				parameters = _provider.GetFor(defaultCommand.Command);
			}

			if (nestedCommands != null)
			{
				commands = GenerateCommands(nestedCommands);
			}

			return new HelpContext()
			{
				Options = options,
				Parameters = parameters ?? Enumerable.Empty<ParameterContext>(),
				NestedCommands = commands ?? Enumerable.Empty<CommandAttribute>(),
			};
		}

		/// <summary>
		/// Generates the <see cref="CommandHelpContext"/> for the specified command type.
		/// </summary>
		/// <param name="type">This is expected to be a CommandBase.</param>
		/// <exception cref="InvalidOperationException">The type is not a CommandBase.</exception>
		public CommandHelpContext GenerateForCommand(Type type)
		{
			if (!typeof(CommandBase).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("type should be a CommandBase.");
			}

			var metadata = MetadataProviders.Current.GetFor(type);
			var attribute = metadata.Attributes.FirstOrDefaultOfRealType<CommandAttribute>();
			var nestedCommands = metadata.Attributes.FirstOrDefaultOfRealType<IncludeCommandsAttribute>();

			var parameters = default(IEnumerable<ParameterContext>);
			var commands = default(IEnumerable<CommandAttribute>);

			parameters = _provider.GetFor(type);

			if (nestedCommands != null)
			{
				commands = GenerateCommands(nestedCommands);
			}

			return new CommandHelpContext()
			{
				Attribute = attribute,
				Parameters = parameters ?? Enumerable.Empty<ParameterContext>(),
				NestedCommands = commands ?? Enumerable.Empty<CommandAttribute>(),
			};
		}

		private static IEnumerable<CommandAttribute> GenerateCommands(IncludeCommandsAttribute nestedCommands)
			=> nestedCommands
			.Commands
			.Select(c => c.GetCustomAttributes(typeof(CommandAttribute), false)[0])
			.OfType<CommandAttribute>()
			.ToArray();
	}
}
