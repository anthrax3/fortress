using System;
using System.Collections;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;
using Castle.Windsor.Tests.Components;
using Xunit;

namespace Castle.Windsor.Tests
{
	public class ABLoaderWithGuardClause : ILazyComponentLoader
	{
		public bool CanLoadNow { get; set; }

		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			Assert.True(CanLoadNow);

			if (service == typeof(A) || service == typeof(B))
				return Component.For(service);
			return null;
		}
	}
}