namespace Castle.Core.Tests.GenInterfaces
{
	public class GenInterfaceImpl<T> : GenInterface<T> where T : new()
	{
		public T DoSomething(T t)
		{
			return t;
		}
	}
}