using Konsola.Metadata;

namespace Konsola.Parser
{
	public class PropertyTarget
	{
		public PropertyTarget(object obj)
		{
			Object = obj;
		}

		public object Object { get; private set; }
		public PropertyMetadata Metadata { get; set; }
		public ParameterAttribute Attribute { get; set; }
		public bool IsSet { get; private set; }

		public void SetValue(object value)
		{
			IsSet = true;
			Metadata.ClrInfo.SetValue(Object, value, null);
		}
	}
}