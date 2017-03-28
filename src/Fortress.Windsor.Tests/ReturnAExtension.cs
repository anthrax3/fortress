using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.Windsor.Tests.Components;

namespace Castle.Windsor.Tests
{
	public class ReturnAExtension : IResolveExtension
	{
		private readonly A a;
		private readonly bool proceed;

		public ReturnAExtension(A a, bool proceed = false)
		{
			this.a = a;
			this.proceed = proceed;
		}

		public void Init(IKernel kernel, IHandler handler)
		{
		}

		public void Intercept(ResolveInvocation invocation)
		{
			if (proceed)
				invocation.Proceed();
			invocation.ResolvedInstance = a;
		}
	}
}