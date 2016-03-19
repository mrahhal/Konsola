using System;

namespace Konsola.Metadata
{
	public class MetadataTypeException : Exception
	{
		public MetadataTypeException()
		{
		}

		public MetadataTypeException(string message) : base(message)
		{
		}

		public MetadataTypeException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
