using System;

namespace Konsola.Metadata
{
	public interface IMetadataProvider
	{
		ObjectMetadata GetFor(Type type);
	}

	public static class MetadataProviderExtensions
	{
		public static ObjectMetadata GetFor<T>(this IMetadataProvider @this)
		{
			return @this.GetFor(typeof(T));
		}
	}
}