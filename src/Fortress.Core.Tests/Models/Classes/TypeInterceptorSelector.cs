using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace Castle.Core.Tests
{
	internal class TypeInterceptorSelector<TInterceptor> : IInterceptorSelector where TInterceptor : IInterceptor
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			var interceptorsOfT = new List<IInterceptor>();
			foreach (var interceptor in interceptors)
				if (interceptor is TInterceptor)
					interceptorsOfT.Add(interceptor);
			return interceptorsOfT.ToArray();
		}

		#endregion
	}
}