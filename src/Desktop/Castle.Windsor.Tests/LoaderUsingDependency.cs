using System;
using System.Collections;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;

namespace Castle.Windsor.Tests
{
	public class LoaderUsingDependency : ILazyComponentLoader
	{
		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			return Component.For(service).DependsOn(arguments);
		}
	}
}