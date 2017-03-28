using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;

namespace Castle.Windsor.Tests
{
	public class CollectItemsExtension : IResolveExtension, IReleaseExtension
	{
		public IList<object> ReleasedItems { get; } = new List<object>();

		public IList<object> ResolvedItems { get; } = new List<object>();

		public void Intercept(ReleaseInvocation invocation)
		{
			invocation.Proceed();
			ReleasedItems.Add(invocation.Instance);
		}

		public void Init(IKernel kernel, IHandler handler)
		{
		}

		public void Intercept(ResolveInvocation invocation)
		{
			invocation.Proceed();
			ResolvedItems.Add(invocation.ResolvedInstance);
		}
	}
}