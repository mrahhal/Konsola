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

		public static IEnumerable<PropertyContext> GetPropertyContexts(this Type @this)
		{
			return @this
				.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Select(prop => new PropertyContext(prop, prop.GetCustomAttribute<ParameterAttribute>()))
				.Where(propc => propc.Attribute != null);
		}

		public static IEnumerable<CommandContext> GetCommandContexts(this Type @this, string commandName)
		{
			return @this
				.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Public)
				.Select(t => new CommandContext(t, t.GetCustomAttribute<CommandAttribute>()))
				.Where(cc => cc.Attribute != null && cc.Attribute.Name == commandName);
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

		public static T GetCustomAttribute<T>(this ParameterInfo @this) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(@this, typeof(T));
		}
	}
}