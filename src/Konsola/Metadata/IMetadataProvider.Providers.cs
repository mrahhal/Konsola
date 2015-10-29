namespace Konsola.Metadata
{
	public static class MetadataProviders
	{
		private static IMetadataProvider _instance;

		public static IMetadataProvider Current
		{
			get { return _instance ?? (_instance = new CachedMetadataProvider()); }
			set { _instance = value; }
		}
	}
}