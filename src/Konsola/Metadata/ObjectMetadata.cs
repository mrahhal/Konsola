using System;
using System.Linq;
using System.Collections.Generic;

namespace Konsola.Metadata
{
	public class ObjectMetadata
	{
		public ObjectMetadata(
			Type type,
			IEnumerable<PropertyMetadata> properties,
			IEnumerable<AttributeMetadata> attributes)
		{
			Type = type;
			Properties = properties;
			Attributes = attributes;
		}

		public Type Type { get; private set; }

		public virtual IEnumerable<PropertyMetadata> Properties { get; private set; }

		public virtual IEnumerable<AttributeMetadata> Attributes { get; private set; }

		public IEnumerable<T> AttributesOfType<T>()
		{
			return
				Attributes
				.Select(a => a.Attribute)
				.OfType<T>();
		}

		public T AttributeOfType<T>()
		{
			return
				AttributesOfType<T>()
				.FirstOrDefault();
		}
	}
}