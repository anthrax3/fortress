using Castle.Core.Configuration;

namespace Castle.MicroKernel.Registration
{
	public class CompoundChild : Node
	{
		private readonly Node[] childNodes;

		internal CompoundChild(string name, Node[] childNodes)
			: base(name)
		{
			this.childNodes = childNodes;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name);
			foreach (var childNode in childNodes)
				childNode.ApplyTo(node);
			configuration.Children.Add(node);
		}
	}
}