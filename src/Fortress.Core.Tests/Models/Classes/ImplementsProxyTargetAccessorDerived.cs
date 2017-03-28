using System;
using Castle.DynamicProxy;

namespace Castle.Core.Tests
{
	public class ImplementsProxyTargetAccessorDerived : IProxyTargetAccessorDerived
	{
		#region IProxyTargetAccessorDerived Members

		public object DynProxyGetTarget()
		{
			throw new NotImplementedException();
		}

		public IInterceptor[] GetInterceptors()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}