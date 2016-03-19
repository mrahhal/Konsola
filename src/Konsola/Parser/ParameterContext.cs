using System.Reflection;
using Konsola.Metadata;

namespace Konsola.Parser
{
	public class ParameterContext
	{
		public ParameterKind Kind { get; set; }
		public PropertyInfo PropertyInfo { get; set; }
		public PropertyMetadata Metadata { get; set; }
		public ParameterAttribute Attribute { get; set; }
		public string FullName { get; set; }
	}
}
