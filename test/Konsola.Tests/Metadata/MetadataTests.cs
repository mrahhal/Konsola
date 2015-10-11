using Konsola.Metadata;
using Konsola.Parsing;
using System;
using System.Linq;
using Xunit;

namespace Konsola.Tests.Metadata
{
	public class MetadataTests
	{
		[Fact]
		public void GetFor()
		{
			var provider = new CachedMetadataProvider();
			var metadata = provider.GetFor<SomeCommand>();

			Assert.NotNull(metadata);
		}

		[Fact]
		public void GetFor_CachesInstances()
		{
			var provider = new CachedMetadataProvider();
			var metadata1 = provider.GetFor<SomeCommand>();
			var metadata2 = provider.GetFor<SomeCommand>();

			Assert.NotNull(metadata1);
			Assert.NotNull(metadata2);
			Assert.Same(metadata1, metadata2);
		}

		[Fact]
		public void GetFor_PopulatesAttributes()
		{
			var provider = new CachedMetadataProvider();
			var metadata = provider.GetFor<SomeCommand>();

			Assert.NotNull(metadata.Attributes);
			Assert.Equal(1, metadata.Attributes.Count());
			Assert.IsType<CommandAttribute>(metadata.Attributes.ElementAt(0).Attribute);
		}

		[Fact]
		public void GetFor_PopulatesProperties()
		{
			var provider = new CachedMetadataProvider();
			var metadata = provider.GetFor<SomeCommand>();

			Assert.NotNull(metadata.Properties);
			Assert.Equal(2, metadata.Properties.Count());
		}

		[Fact]
		public void GetFor_PopulatesAttributesInProperties()
		{
			var provider = new CachedMetadataProvider();
			var metadata = provider.GetFor<SomeCommand>();

			Assert.Equal(1, metadata.Properties.ElementAt(0).Attributes.Count());
			Assert.IsType<ParameterAttribute>(metadata.Properties.ElementAt(0).Attributes.ElementAt(0).Attribute);
		}

		[Fact]
		public void GetFor_SubType_CorrectlyReadsValuesFromBaseType()
		{
			var provider = new CachedMetadataProvider();
			var metadata = provider.GetFor<SubCommand>();

			Assert.Equal(0, metadata.Attributes.Count());
			Assert.Equal(3, metadata.Properties.Count());
			Assert.Equal(1, metadata.Properties.Where(p => p.ClrInfo.Name == "Baz").First().Attributes.Count());
		}
	}

	[Command("Foo", Description = "Bar")]
	public class SomeCommand
	{
		[Parameter("sm")]
		public int Foo { get; set; }

		public string Bar { get; set; }
	}

	public class SubCommand : SomeCommand
	{
		[Parameter("b")]
		public string Baz { get; set; }
	}
}