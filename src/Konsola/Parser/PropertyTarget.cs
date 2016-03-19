using Konsola.Metadata;

namespace Konsola.Parser
{
	public class PropertyTarget
	{
		public PropertyTarget(object obj, ParameterContext parameterContext)
		{
			Object = obj;
			ParameterContext = parameterContext;
		}

		public object Object { get; private set; }
		public ParameterContext ParameterContext { get; private set; }
		public bool IsSet { get; private set; }

		public PropertyMetadata Metadata { get { return ParameterContext.Metadata; } }
		public ParameterAttribute Attribute { get { return ParameterContext.Attribute; } }

		public void SetValue(object value)
		{
			IsSet = true;
			Metadata.ClrInfo.SetValue(Object, value, null);
		}
	}
}
