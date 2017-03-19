namespace Castle.Core.Tests.GenInterfaces
{
	public class GenInterfaceWithGenMethodsImpl<T> : GenInterfaceWithGenMethods<T>
	{
		public void DoSomething<Z>(Z z, T t)
		{
		}
	}
}