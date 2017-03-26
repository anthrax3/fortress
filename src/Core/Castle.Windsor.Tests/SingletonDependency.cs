using Castle.Windsor.Core;

namespace Castle.Windsor.Tests
{
	[Singleton]
	public class SingletonDependency
	{
		public SingletonDependency(SingletonComponent c)
		{
		}
	}
}