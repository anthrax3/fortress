using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor.Tests.Components;

namespace Castle.Windsor.Tests
{
	public class LoaderWithDependency : ILazyComponentLoader
	{
		private IEmployee employee;

		public LoaderWithDependency(IEmployee employee)
		{
			this.employee = employee;
		}

		public IRegistration Load(string name, Type service, IDictionary arguments)
		{
			return null;
		}
	}
}