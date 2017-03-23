using Castle.Windsor.MicroKernel.Registration;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Descriptors
{
	public class AttributeKeyDescriptor<S>
		where S : class
	{
		private readonly ComponentRegistration<S> component;
		private readonly string name;

		public AttributeKeyDescriptor(ComponentRegistration<S> component, string name)
		{
			this.component = component;
			this.name = name;
		}

		public ComponentRegistration<S> Eq(object value)
		{
			var attribValue = value != null ? value.ToString() : "";
			return component.AddAttributeDescriptor(name, attribValue);
		}
	}
}