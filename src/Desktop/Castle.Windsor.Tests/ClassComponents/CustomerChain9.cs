using System;

namespace Castle.Windsor.Tests.ClassComponents
{
	public class CustomerChain9 : CustomerChain1
	{
		public CustomerChain9(ICustomer customer) : base(customer)
		{
		}
	}
}