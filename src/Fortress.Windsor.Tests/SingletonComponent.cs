using Castle.Core;

namespace Castle.Windsor.Tests
{
	[Singleton]
	public class SingletonComponent
	{
		public static int CtorCallsCount;

		public SingletonComponent()
		{
			CtorCallsCount++;
		}

		public SingletonDependency Dependency { get; set; }
	}
}