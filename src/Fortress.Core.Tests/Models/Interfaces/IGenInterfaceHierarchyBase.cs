namespace Castle.Core.Tests.GenInterfaces
{
	public interface IGenInterfaceHierarchyBase<T>
	{
		void Add();

		void Add(T item);

		T Get();

		T[] GetAll();
	}
}