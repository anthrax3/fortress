using System;
using System.Collections;
using System.Threading;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;

namespace Castle.Windsor.Tests
{
	public class SlowLoader : ILazyComponentLoader
	{
		public IRegistration Load(string name, Type service, IDictionary argume)
		{
			Thread.Sleep(200);
			return Component.For(service).Named(name);
		}
	}
}