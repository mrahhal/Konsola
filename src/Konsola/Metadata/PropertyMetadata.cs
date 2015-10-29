using System;
using System.Collections.Generic;
using System.Reflection;

namespace Konsola.Metadata
{
	public class PropertyMetadata
	{
		public PropertyMetadata(
			PropertyInfo pi,
			IEnumerable<AttributeMetadata> attributes)
		{
			Type = pi.PropertyType;
			ClrInfo = pi;
			Attributes = attributes;
		}

		public Type Type { get; private set; }

		public PropertyInfo ClrInfo { get; private set; }

		public IEnumerable<AttributeMetadata> Attributes { get; private set; }

		public T GetValue<T>(object obj)
		{
			ValidateType(typeof(T));
			return (T)ClrInfo.GetValue(obj, null);
		}

		public void SetValue<T>(object obj, T value)
		{
			ValidateType(typeof(T));
			ClrInfo.SetValue(obj, value, null);
		}

		private void ValidateType(Type type)
		{
			if (type != Type)
			{
				throw new MetadataTypeException();
			}
		}
	}
}