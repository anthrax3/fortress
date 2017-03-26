using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class ComplexChild : Node
	{
		private readonly IConfiguration configNode;

		internal ComplexChild(string name, IConfiguration configNode)
			: base(name)
		{
			this.configNode = configNode;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			node.Children.Add(configNode);
			configuration.Children.Add(node);
		}
	}
}