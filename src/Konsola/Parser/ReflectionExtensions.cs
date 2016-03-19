using System;
using System.Reflection;
using Konsola.Metadata;

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
			=> (T)Activator.CreateInstance(@this);

		public static bool IsAttributeDefined<T>(this MemberInfo @this) where T : Attribute
			=> Attribute.IsDefined(@this, typeof(T));

		public static bool IsCommandType(this Type @this)
			=> typeof(CommandBase).IsAssignableFrom(@this);
	}
}
