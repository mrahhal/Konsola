using System.Reflection;

namespace Konsola.Parser
{
	public class ParameterContext
	{
		public ParameterKind Kind { get; set; }
		public PropertyInfo PropertyInfo { get; set; }
		public ParameterAttribute Attribute { get; set; }
		public string FullName { get; set; }
	}
}