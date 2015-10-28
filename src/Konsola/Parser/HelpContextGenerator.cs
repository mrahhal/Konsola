using Konsola.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Konsola.Parser
{
	public static class HelpContextGenerator
	{
		/// <summary>
		/// Generates the <see cref="HelpContext"/> for the specified context type.
		/// </summary>
		/// <param name="type">This is expected to be a ContextBase.</param>
		/// <exception cref="InvalidOperationException">The type is not a ContextBase.</exception>
		public static HelpContext Generate(Type type, Tokenizer tokenizer)
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
				parameters = GenerateParameters(defaultCommand.Command, tokenizer);
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
		public static CommandHelpContext GenerateForCommand(Type type, Tokenizer tokenizer)
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

			parameters = GenerateParameters(type, tokenizer);

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

		private static IEnumerable<ParameterContext> GenerateParameters(Type type, Tokenizer tokenizer)
		{
			var metadata = MetadataProviders.Current.GetFor(type);
			return
				metadata
				.Properties
				.Select(p => new
				{
					Property = p,
					Attribute = p.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>(),
					Kind = GetKindForProperty(p)
				})
				.Where(pa => pa.Attribute != null)
				.Select(pa => new ParameterContext()
				{
					PropertyInfo = pa.Property.ClrInfo,
					Attribute = pa.Attribute,
					Kind = pa.Kind,
					FullName = GetFullName(pa.Property, pa.Kind, tokenizer)
				})
				.ToArray();
		}

		private static IEnumerable<CommandAttribute> GenerateCommands(IncludeCommandsAttribute nestedCommands)
		{
			return
				nestedCommands
				.Commands
				.Select(c => c.GetCustomAttributes(typeof(CommandAttribute), false)[0])
				.OfType<CommandAttribute>()
				.ToArray();
		}

		private static string GetFullName(PropertyMetadata propertyMetadata, ParameterKind parameterKind, Tokenizer tokenizer)
		{
			var del = string.Empty;
			switch (parameterKind)
			{
				case ParameterKind.String:
				case ParameterKind.Int:
				case ParameterKind.Enum:
					del = tokenizer.OptionPre;
					break;
				case ParameterKind.Bool:
					del = tokenizer.SwitchPre;
					break;
				default:
					break;
			}
			var names = propertyMetadata.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>().Names;
			return names
				.Split(',')
				.Select(n => del + n)
				.Aggregate((s1, s2) => s1 + "," + s2);
		}

		private static ParameterKind GetKindForProperty(PropertyMetadata p)
		{
			if (p.Type == typeof(string))
				return ParameterKind.String;
			else if (p.Type == typeof(int))
				return ParameterKind.Int;
			else if (p.Type == typeof(bool))
				return ParameterKind.Bool;
			else if (p.Type.IsEnum)
				return ParameterKind.Enum;
			else
				return ParameterKind.None;
		}
	}
}