using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class Attrib : Node
	{
		private readonly string value;

		internal Attrib(string name, string value)
			: base(name)
		{
			this.value = value;
		}

		public override void ApplyTo(IConfiguration configuration)
		{
			configuration.Attributes.Add(Name, value);
		}

		public static NamedAttribute ForName(string name)
		{
			return new NamedAttribute(name);
		}
	}
}