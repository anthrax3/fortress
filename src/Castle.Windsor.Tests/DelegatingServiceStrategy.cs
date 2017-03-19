using System;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.Handlers;

namespace Castle.Windsor.Tests
{
	internal class DelegatingServiceStrategy : IGenericServiceStrategy
	{
		private readonly Func<Type, ComponentModel, bool> supports;

		public DelegatingServiceStrategy(Func<Type, ComponentModel, bool> supports)
		{
			this.supports = supports;
		}

		public bool Supports(Type service, ComponentModel component)
		{
			return supports.Invoke(service, component);
		}
	}
}