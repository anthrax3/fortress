using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.Context;
using Castle.Windsor.MicroKernel.Handlers;

namespace CastleTests.TestImplementationsOfExtensionPoints
{
	using System;

	using Castle.Core;

	public class UseStringGenericStrategy : IGenericImplementationMatchingStrategy
	{
		public Type[] GetGenericArguments(ComponentModel model, CreationContext context)
		{
			return new[] { typeof(string) };
		}
	}
}
