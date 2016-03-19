using System;
using System.Collections.Concurrent;
using System.Linq;
using Konsola.Metadata;

namespace Konsola.Parser
{
	public class ParameterContextProvider
	{
		private ConcurrentDictionary<Type, ParameterContext[]> _cache =
			new ConcurrentDictionary<Type, ParameterContext[]>();

		private Tokenizer _tokenizer;

		public ParameterContextProvider(Tokenizer tokenizer)
		{
			_tokenizer = tokenizer;
		}

		public ParameterContext[] GetFor(Type type)
		{
			return _cache.GetOrAdd(type, (t) =>
				{
					return CreateParameterContexts(MetadataProviders.Current.GetFor(t));
				});
		}

		private ParameterContext[] CreateParameterContexts(ObjectMetadata commandMetadata)
		{
			return
				commandMetadata
				.Properties
				.Where(p => p.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>() != null)
				.Select(p => new ParameterContext()
				{
					PropertyInfo = p.ClrInfo,
					Attribute = p.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>(),
					Metadata = p,
					Kind = GetKindForProperty(p),
					FullName = GetFullName(p, GetKindForProperty(p))
				})
				.ToArray();
		}

		private string GetFullName(PropertyMetadata propertyMetadata, ParameterKind parameterKind)
		{
			var del = string.Empty;
			switch (parameterKind)
			{
				case ParameterKind.String:
				case ParameterKind.Int:
				case ParameterKind.Enum:
					del = _tokenizer.OptionPre;
					break;

				case ParameterKind.Bool:
					del = _tokenizer.SwitchPre;
					break;

				default:
					break;
			}
			var names = propertyMetadata.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>().Names;
			return names
				.Split(',')
				.Select(n => del + n)
				.Aggregate((s1, s2) => s1 + "," + s2);
		}

		private ParameterKind GetKindForProperty(PropertyMetadata p)
		{
			if (p.Type == typeof(string))
				return ParameterKind.String;
			else if (p.Type == typeof(string[]))
				return ParameterKind.StringArray;
			else if (p.Type == typeof(int))
				return ParameterKind.Int;
			else if (p.Type == typeof(bool))
				return ParameterKind.Bool;
			else if (p.Type.IsEnum)
				return ParameterKind.Enum;
			else
				return ParameterKind.None;
		}
	}
}
