using System;

namespace Konsola.Metadata
{
	public class AttributeMetadata
	{
		public AttributeMetadata(Attribute attribute)
		{
			Type = attribute.GetType();
			Attribute = attribute;
		}

		public Type Type { get; }

		public Attribute Attribute { get; }
	}
}
