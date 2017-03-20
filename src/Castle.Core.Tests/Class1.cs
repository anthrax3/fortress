namespace Castle.Core.Tests
{
	public class Class1 : IGenericInterface
	{
		public T GenericMethod<T>()
		{
			return default(T);
		}
	}
}