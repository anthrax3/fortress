namespace Castle.Core.Tests
{
	public interface IMultiGenericInterface
	{
		T1 Method<T1, T2>(T2 p);

		T2 Method<T1, T2>(T1 p);
	}
}