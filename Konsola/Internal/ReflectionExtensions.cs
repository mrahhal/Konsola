//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Konsola.Internal
{
	internal static class ReflectionExtensions
	{
		private const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

		public static IEnumerable<ParameterContext> GetPropertyContexts(this Type @this)
		{
			return @this
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Select(prop => new ParameterContext(prop))
				.Where(propc => propc.ParameterAttribute != null);
		}

		public static CommandContext GetCommandContextOrDefault(this Type @this, string commandName)
		{
			var includeCommandsAttribute = @this.GetCustomAttribute<IncludeCommandsAttribute>();
			if (includeCommandsAttribute == null)
			{
				return null;
			}

			return includeCommandsAttribute
				.Commands
				.Select(t => new CommandContext(t))
				.Where(cc => cc.Attribute != null && cc.Attribute.Name == commandName)
				.FirstOrDefault();
		}

		public static T CreateInstance<T>(this Type @this)
		{
			return (T)Activator.CreateInstance(@this);
		}

		public static T GetCustomAttribute<T>(this Assembly @this) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(@this, typeof(T));
		}

		public static T GetCustomAttribute<T>(this Module @this) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(@this, typeof(T));
		}

		public static T GetCustomAttribute<T>(this MemberInfo @this) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(@this, typeof(T));
		}

		public static T[] GetCustomAttributes<T>(this MemberInfo @this) where T : Attribute
		{
			return Attribute.GetCustomAttributes(@this, typeof(T)).OfType<T>().ToArray();
		}

		public static T GetCustomAttribute<T>(this ParameterInfo @this) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(@this, typeof(T));
		}

		public static bool IsCommandType(this Type @this)
		{
			return typeof(CommandBase).IsAssignableFrom(@this);
		}
	}
}