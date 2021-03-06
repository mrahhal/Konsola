﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Konsola.Metadata
{
	public class CachedMetadataProvider : IMetadataProvider
	{
		private ConcurrentDictionary<Type, ObjectMetadata> _cache = new ConcurrentDictionary<Type, ObjectMetadata>();

		public ObjectMetadata GetFor(Type type)
		{
			return _cache.GetOrAdd(type, GenerateNew);
		}

		public ObjectMetadata GenerateNew(Type type)
		{
			var properties = GenerateProperties(type).ToArray();
			var attributes = GenerateAttributes(type).ToArray();

			return new ObjectMetadata(type, properties, attributes);
		}

		private IEnumerable<PropertyMetadata> GenerateProperties(Type type)
		{
			return GetDeclaredPropertiesInType(type)
				.Select(pi => GenerateForProperty(pi));
		}

		private IEnumerable<PropertyInfo> GetDeclaredPropertiesInType(Type type)
		{
			if (type == null)
				yield break;
			foreach (var pi in GetDeclaredPropertiesInType(type.BaseType))
			{
				yield return pi;
			}
			foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
			{
				yield return pi;
			}
		}

		private IEnumerable<AttributeMetadata> GenerateAttributes(Type type)
		{
			return type
				.GetCustomAttributes(false)
				.Select(a => GenerateForAttribute((Attribute)a));
		}

		private IEnumerable<AttributeMetadata> GenerateAttributes(MemberInfo mi)
		{
			return mi
				.GetCustomAttributes(false)
				.Select(a => GenerateForAttribute((Attribute)a));
		}

		public PropertyMetadata GenerateForProperty(PropertyInfo pi)
		{
			return new PropertyMetadata(pi, GenerateAttributes(pi).ToArray());
		}

		public AttributeMetadata GenerateForAttribute(Attribute attribute)
		{
			return new AttributeMetadata(attribute);
		}
	}
}