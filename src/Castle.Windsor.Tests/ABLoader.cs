using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor.Tests.Components;

namespace Castle.Windsor.Tests
{
    public class ABLoader : ILazyComponentLoader
	{
		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			if (service == typeof(A) || service == typeof(B))
				return Component.For(service);
			return null;
		}
	}
}