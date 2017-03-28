namespace Castle.Core.Tests
{
	public class MultiGenericClass : IMultiGenericInterface
	{
		public T1 Method<T1, T2>(T2 p)
		{
			return default(T1);
		}

		public T2 Method<T1, T2>(T1 p)
		{
			return default(T2);
		}
	}
}