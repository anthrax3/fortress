namespace Castle.Core.Tests.GenInterfaces
{
	public class OnlyGenMethodsInterfaceImpl : OnlyGenMethodsInterface
	{
		public Z DoSomething<Z>(Z z) where Z : new()
		{
			return z;
		}
	}
}