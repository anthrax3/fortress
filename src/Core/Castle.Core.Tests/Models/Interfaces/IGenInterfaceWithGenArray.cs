namespace Castle.Core.Tests.GenInterfaces
{
	public interface IGenInterfaceWithGenArray<T>
		where T : struct
	{
		void CopyTo(T[] items);

		T[] CreateItems();
	}
}