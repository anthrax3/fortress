using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Castle.Core.Core.Internal;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;

namespace Castle.Windsor.Tests
{
	public class LoaderForDefaultImplementations : ILazyComponentLoader
	{
		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			if (!service.GetAttributes<DefaultImplementationAttribute>().Any())
				return null;

			var attributes = service.GetTypeInfo().GetCustomAttributes(typeof(DefaultImplementationAttribute), false).ToArray();
			var attribute = attributes[0] as DefaultImplementationAttribute;
			return Component.For(service).ImplementedBy(attribute.Implementation).Named(name);
		}
	}
}