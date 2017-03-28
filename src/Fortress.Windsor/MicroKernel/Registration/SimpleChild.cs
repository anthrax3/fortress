using Castle.Core.Configuration;

namespace Castle.MicroKernel.Registration
{
	public class SimpleChild : Node
	{
		private readonly string value;

		internal SimpleChild(string name, string value)
			: base(name)
		{
			this.value = value;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			var node = new MutableConfiguration(Name, value);
			configuration.Children.Add(node);
		}
	}
}