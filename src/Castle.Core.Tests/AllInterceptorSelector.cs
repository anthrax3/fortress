using System;
using System.Reflection;
using Castle.Core.DynamicProxy;

namespace Castle.Core.Tests
{
	[Serializable]
	public class AllInterceptorSelector : IInterceptorSelector
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			return interceptors;
		}

		#endregion
	}
}