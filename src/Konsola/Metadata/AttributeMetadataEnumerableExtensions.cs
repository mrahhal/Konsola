using System;
using System.Collections.Generic;
using System.Linq;

namespace Konsola.Metadata
{
	public static class AttributeMetadataEnumerableExtensions
	{
		public static IEnumerable<T> OfRealType<T>(this IEnumerable<AttributeMetadata> @this)
		{
			return
				@this
				.Select(a => a.Attribute)
				.OfType<T>();
		}

		public static T FirstOrDefaultOfRealType<T>(this IEnumerable<AttributeMetadata> @this)
		{
			return
				@this
				.OfRealType<T>()
				.FirstOrDefault();
		}
	}
}