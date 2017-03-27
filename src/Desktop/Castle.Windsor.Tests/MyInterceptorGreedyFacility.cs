using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Castle.Windsor.Tests
{
	public class MyInterceptorGreedyFacility : IFacility
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
			if (key == "key")
				handler.ComponentModel.Interceptors.Add(
					new InterceptorReference("interceptor"));
		}
	}
}