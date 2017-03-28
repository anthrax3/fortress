using System.Collections.Generic;

namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public abstract class MyOwnClass
	{
		public virtual void Foo<T>(List<T>[] action)
		{
		}

		/* ... */
	}
}