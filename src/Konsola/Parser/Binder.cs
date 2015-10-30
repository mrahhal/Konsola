﻿using Konsola.Metadata;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Konsola.Parser
{
	public static class Binder
	{
		public static void Bind(BindingContext c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (c.Sources == null || c.Targets == null)
			{
				throw new InvalidOperationException();
			}

			var sources = c.Sources;
			var targets = c.Targets;

			if (sources.Length == 0 || targets.Length == 0)
			{
				return;
			}
			ValidateSources(sources);

			if (sources[0].Kind != RawTokenKind.Raw)
			{
				// The first source is not raw so no need to think about positional arguments
				BindNormal(sources, targets);
				return;
			}

			var orderedTargets =
				targets
				.OrderBy(t => t.Attribute.Position)
				.ToArray();
			BindWithPositionalArguments(sources, orderedTargets);
		}

		private static void BindNormal(DataSource[] sources, PropertyTarget[] targets)
		{
			foreach (var source in sources)
			{
				MatchAndSet(source, targets);
			}
		}

		private static void BindWithPositionalArguments(DataSource[] sources, PropertyTarget[] targets)
		{
			var i = 0;
			for (; i < sources.Length; i++)
			{
				var source = sources[i];
				if (source.Kind != RawTokenKind.Raw)
				{
					break;
				}
				BindTargetFromSource(source, targets[i]);
			}
			for (; i < sources.Length; i++)
			{
				MatchAndSet(sources[i], targets);
			}
		}

		private static void MatchAndSet(DataSource source, PropertyTarget[] targets)
		{
			var target = Match(source, targets);
			if (target == null)
			{
				throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
			}
			BindTargetFromSource(source, target);
		}

		private static void BindTargetFromSource(DataSource source, PropertyTarget target)
		{
			var propertyType = target.Metadata.Type;
			if (propertyType == typeof(int))
			{
				if (source.Kind == RawTokenKind.Switch)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
				}
				int parsed;
				if (!int.TryParse(source.Value, out parsed))
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidValue, source.FullIdentifier);
				}
				target.SetValue(parsed);
			}
			else if (propertyType == typeof(string))
			{
				if (source.Kind == RawTokenKind.Switch)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
				}
				target.SetValue(source.Value);
			}
			else if (propertyType.IsEnum)
			{
				if (source.Kind == RawTokenKind.Switch)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
				}
				BindEnum(source, target);
			}
			else if (propertyType == typeof(bool))
			{
				if (source.Kind != RawTokenKind.Switch)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
				}
				target.SetValue(Boxes.True);
			}
			else if (propertyType == typeof(string[]))
			{
				if (source.Kind == RawTokenKind.Switch)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.FullIdentifier);
				}
				var values = source.Value.Split(',').Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
				target.SetValue(values);
			}
		}

		private static void BindEnum(DataSource source, PropertyTarget target)
		{
			var type = target.Metadata.Type;
			var isFlags = type.IsAttributeDefined<FlagsAttribute>();
			var validValues = GetValidEnumValues(type);
			var value = source.Value;
			if (!isFlags)
			{
				if (value.Contains(",") || !validValues.Contains(value, StringComparer.OrdinalIgnoreCase))
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidValue, source.FullIdentifier);
				}
				var e = Enum.Parse(type, value, true);
				target.SetValue(e);
			}
			else
			{
				var values = value.Split(',');
				int crux = 0;
				foreach (var v in values)
				{
					if (!validValues.Contains(v, StringComparer.OrdinalIgnoreCase))
					{
						throw new CommandLineException(CommandLineExceptionKind.InvalidValue, source.FullIdentifier);
					}
					var e = Enum.Parse(type, v, true);
					crux |= (int)e;
				}
				target.SetValue(crux);
			}
		}

		private static string[] GetValidEnumValues(Type type)
		{
			var sb = new StringBuilder();
			foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				if (sb.Length != 0)
					sb.Append(',');
				sb.Append(fi.Name);
			}
			return sb.ToString().Split(',');
		}

		private static void ValidateSources(DataSource[] sources)
		{
			for (int i = 0; i < sources.Length; i++)
			{
				var source = sources[i];
				if (source.Kind == RawTokenKind.Option && source.Value == null)
				{
					throw new CommandLineException(CommandLineExceptionKind.MissingValue, source.FullIdentifier);
				}
			}
		}

		private static PropertyTarget Match(DataSource source, PropertyTarget[] targets)
		{
			if (source.Kind == RawTokenKind.Raw)
			{
				throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, source.Value);
			}
			var name = source.Identifier;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i].Attribute.Names.Contains(name))
					return targets[i];
			}
			return null;
		}
	}
}