using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class NamedChild : Node
	{
		internal NamedChild(string name)
			: base(name)
		{
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			configuration.Children.Add(node);
		}

		public SimpleChild Eq(string value)
		{
			return new SimpleChild(Name, value);
		}

		public SimpleChild Eq(object value)
		{
			var valueStr = value != null ? value.ToString() : string.Empty;
			return new SimpleChild(Name, valueStr);
		}

		public ComplexChild Eq(IConfiguration configNode)
		{
			return new ComplexChild(Name, configNode);
		}

		public CompoundChild Eq(params Node[] childNodes)
		{
			return new CompoundChild(Name, childNodes);
		}
	}
}