namespace Castle.Core.Tests.GenInterfaces
{
	public class GenInterfaceWithGenMethodsAndGenReturnImpl<T> : GenInterfaceWithGenMethodsAndGenReturn<T>
	{
		public Z DoSomething<Z>(Z z, T t)
		{
			return z;
		}
	}
}