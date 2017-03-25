using System;

namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class ExplicitInterfaceWithPropertyImplementation : ISimpleInterfaceWithProperty
	{
		public int Age
		{
			get { throw new NotImplementedException(); }
		}
	}
}