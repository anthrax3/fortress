using Castle.Core.Configuration;

namespace Castle.MicroKernel.Registration
{
	public class ParameterKey
	{
		internal ParameterKey(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public Parameter Eq(string value)
		{
			return new Parameter(Name, value);
		}

		public Parameter Eq(IConfiguration configNode)
		{
			return new Parameter(Name, configNode);
		}
	}
}