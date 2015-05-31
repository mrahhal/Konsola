//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Linq;
using Konsola.Attributes;
using Xunit;

namespace Konsola.Tests
{
	public class ParsingTests
	{
		[Fact(DisplayName = "Split command line args")]
		public void SplitCommandLineArgsTest()
		{
			var args = "-my some -s2 \"something -int 3\" --sw".SplitCommandLineArgs();

			Assert.True(args.Length == 5);
			Assert.True(args[0] == "-my");
			Assert.True(args[1] == "some");
			Assert.True(args[2] == "-s2");
			Assert.True(args[3] == "something -int 3");
			Assert.True(args[4] == "--sw");
		}

		[Fact(DisplayName= "Basic")]
		public void BasicTest()
		{
			var args = "-my some -s2 something -int 3 --sw".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.True(context.InnerContext == null);
			Assert.True(context.SomeString == "some");
			Assert.True(context.SomeString2 == "something");
			Assert.True(context.SomeInt == 3);
			Assert.True(context.SomeBool == true);
		}

		[Fact(DisplayName = "Command basic")]
		public void CommandBasicTest()
		{
			var args = "restore --an".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.True(context.InnerContext != null);
			Assert.True(context.InnerContext is Context.RestoreContext);
			Assert.True(((Context.RestoreContext)context.InnerContext).Another == true);
		}

		[Fact(DisplayName = "Enum")]
		public void EnumTest()
		{
			var args = "-my some -p windows -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.True(context.Platform == Platform.Windows);
		}

		[Fact(DisplayName = "Invalid enum value should fail")]
		public void InvalidEnumValueShouldFail()
		{
			var args = "-my some -p some -int 3".SplitCommandLineArgs();

			Assert.Throws<ParsingException>(() =>
				{
					KContext.Parse<Context>(args);
				});
		}

		[Fact(DisplayName = "Multiple enum values with enum should fail")]
		public void MultipleEnumValuesWithEnumShouldFail()
		{
			var args = "-my some -p windows,unix -int 3".SplitCommandLineArgs();

			Assert.Throws<ParsingException>(() =>
				{
					KContext.Parse<Context>(args);
				});
		}

		[Fact(DisplayName = "Flags enum")]
		public void FlagsEnumTest()
		{
			var args = "-my some -fp windows,linux -int 3".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.True((context.FlagsPlatform & FlagsPlatform.Windows) == FlagsPlatform.Windows
				&& (context.FlagsPlatform & FlagsPlatform.Linux) == FlagsPlatform.Linux);
		}

		[Fact(DisplayName = "Enum inside command")]
		public void EnumInsideCommandTest()
		{
			var args = "restore -p linux".SplitCommandLineArgs();

			var context = KContext.Parse<Context>(args);

			Assert.True(context.InnerContext != null);
			Assert.True(context.InnerContext is Context.RestoreContext);
			Assert.True(((Context.RestoreContext)context.InnerContext).Plaform == Platform.Linux);
		}

		[Fact(DisplayName = "Invalid flags enum value should fail")]
		public void InvalidFlagsEnumValueShouldFail()
		{
			var args = "-my some -fp windows,some -int 3".SplitCommandLineArgs();

			Assert.Throws<ParsingException>(() =>
			{
				KContext.Parse<Context>(args);
			});
		}

		[Fact(DisplayName = "Should throw if context not valid")]
		public void ShouldThrowIfContextNotValid()
		{
			var args = "-my some".SplitCommandLineArgs();

			Assert.Throws<ContextException>(() =>
			{
				KContext.Parse<FaultyContext>(args);
			});
		}

		[Fact(DisplayName = "Should throw if mandatory param is missing")]
		public void ShouldThrowIfMandantoryParamIsMissing()
		{
			var args = "-my some".SplitCommandLineArgs();

			var ex = Assert.Throws<ParsingException>(() =>
			{
				KContext.Parse<Context>(args);
			});

			Assert.True(ex.Kind == ExceptionKind.MissingParameter);
		}

		[Fact(DisplayName = "Should throw if data is missing")]
		public void ShouldThrowIfDataIsMissing()
		{
			var args = "-my -s2 something -int 3 --sw".SplitCommandLineArgs();

			var ex = Assert.Throws<ParsingException>(() =>
			{
				KContext.Parse<Context>(args);
			});

			Assert.True(ex.Kind == ExceptionKind.MissingValue && ex.Name == "-my");
		}
	}

	public enum Platform
	{
		None,
		Windows,
		Unix,
		Linux,
	}

	[Flags]
	public enum FlagsPlatform
	{
		None = 0,
		Windows = 1,
		Unix = 2,
		Linux = 4,
	}

	[KContextOptions]
	public class Context : KContextBase
	{
		[KParameter("my")]
		public string SomeString { get; set; }

		[KParameter("s1,s2")]
		public string SomeString2 { get; set; }

		[KParameter("int", isMandatory: true)]
		public int SomeInt { get; set; }

		[KParameter("sw")]
		public bool SomeBool { get; set; }

		[KParameter("p")]
		public Platform Platform { get; set; }

		[KParameter("fp")]
		public FlagsPlatform FlagsPlatform { get; set; }

		[KCommand("restore")]
		public class RestoreContext : KContextBase
		{
			[KParameter("an")]
			public bool Another { get; set; }

			[KParameter("p")]
			public Platform Plaform { get; set; }
		}
	}

	[KContextOptions]
	public class FaultyContext : KContextBase
	{
		[KParameter("my not")]
		public string SomeString { get; set; }
	}

	public static partial class Mixin
	{
		public static string[] SplitCommandLineArgs(this string args)
		{
			return KUtils.SplitCommandLineArgs(args).ToArray();
		}
	}
}