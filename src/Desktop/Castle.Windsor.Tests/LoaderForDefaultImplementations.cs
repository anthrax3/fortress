using System;
using System.Collections;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;

namespace Castle.Windsor.Tests
{
	public class LoaderForDefaultImplementations : ILazyComponentLoader
	{
		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			if (!Attribute.IsDefined(service, typeof(DefaultImplementationAttribute)))
				return null;

			var attributes = service.GetCustomAttributes(typeof(DefaultImplementationAttribute), false);
			var attribute = attributes[0] as DefaultImplementationAttribute;
			return Component.For(service).ImplementedBy(attribute.Implementation).Named(name);
		}
	}
}