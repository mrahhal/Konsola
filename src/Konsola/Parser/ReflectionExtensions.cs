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