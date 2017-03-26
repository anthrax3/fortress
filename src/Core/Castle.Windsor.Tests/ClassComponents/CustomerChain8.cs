using System;

namespace Castle.Windsor.Tests.ClassComponents
{
	public class CustomerChain8 : CustomerChain1
	{
		public CustomerChain8(ICustomer customer) : base(customer)
		{
		}
	}
}