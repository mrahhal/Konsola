using Konsola.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Konsola.Parser
{
	internal static class ReflectionExtensions
	{
		private const BindingFlags Declared =
			BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance |
			BindingFlags.Static |
			BindingFlags.DeclaredOnly;

		public static ObjectMetadata FindCommandMetadataWithName(this ObjectMetadata @this, string name)
		{
			var includes = @this
				.Attributes
				.FirstOrDefaultOfRealType<IncludeCommandsAttribute>();
			if (includes == null)
			{
				return null;
			}
			foreach (var command in includes.Commands)
			{
				var metadata = MetadataProviders.Current.GetFor(command);
				var commandAttribute = metadata
					.Attributes
					.FirstOrDefaultOfRealType<CommandAttribute>();
				if (commandAttribute == null)
				{
					continue;
				}
				if (string.Equals(commandAttribute.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return metadata;
				}
			}
			return null;
		}

		public static T CreateInstance<T>(this Type @this)
		{
			return (T)Activator.CreateInstance(@this);
		}

		public static bool IsCommandType(this Type @this)
		{
			return typeof(CommandBase).IsAssignableFrom(@this);
		}
	}
}