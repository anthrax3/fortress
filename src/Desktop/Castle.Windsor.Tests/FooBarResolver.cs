using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Context;

namespace Castle.Windsor.Tests
{
	public class FooBarResolver : ISubDependencyResolver
	{
		public int? Result;

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return Result != null;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return Result.Value;
		}
	}
}