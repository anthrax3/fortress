using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;

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