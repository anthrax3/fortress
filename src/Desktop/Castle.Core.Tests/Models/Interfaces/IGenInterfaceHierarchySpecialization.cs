namespace Castle.Core.Tests.GenInterfaces
{
	public interface IGenInterfaceHierarchySpecialization<T> : IGenInterfaceHierarchyBase<T>
	{
		int Count();

		T[] FetchAll();
	}
}