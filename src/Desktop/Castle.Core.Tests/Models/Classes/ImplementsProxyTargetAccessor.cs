using System;
using Castle.DynamicProxy;

namespace Castle.Core.Tests
{
	public class ImplementsProxyTargetAccessor : IProxyTargetAccessor
	{
		#region IProxyTargetAccessor Members

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