using System.Linq;
using Castle.Core.Core.Configuration;
using Castle.Core.DynamicProxy;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel;

namespace Castle.Windsor.Tests
{
	public class MyInterceptorGreedyFacility2 : IFacility
	{
		public void Terminate()
		{
		}

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			kernel.ComponentRegistered += OnComponentRegistered;
		}

		private void OnComponentRegistered(string key, IHandler handler)
		{
			if (handler.ComponentModel.Services.Any(s => ReflectionUtil.Is<IInterceptor>(s)))
				return;

			handler.ComponentModel.Interceptors.Add(new InterceptorReference("interceptor"));
		}
	}
}