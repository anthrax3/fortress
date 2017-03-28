namespace Castle.Core.Tests
{
	public interface IRepository<TEntity, TKey>
	{
		TEntity GetById(TKey key);
	}
}