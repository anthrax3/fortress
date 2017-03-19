using Castle.Windsor.Core;

namespace Castle.Windsor.Tests
{
	[Transient]
	public class FooBarNonValue
	{
		public FooBarNonValue(Tester1 test1)
		{
		}

		public FooBarNonValue(Tester2 test2)
		{
		}
	}
}