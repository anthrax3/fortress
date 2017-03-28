namespace Castle.Core.Tests
{
	public class Class2 : IGenericInterface
	{
		public T GenericMethod<T>()
		{
			return default(T);
		}
	}
}