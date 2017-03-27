using Castle.Core;

namespace Castle.Windsor.Tests
{
	[Singleton]
	public class SingletonPropertyComponent
	{
		public static int CtorCallsCount;

		public SingletonPropertyComponent()
		{
			CtorCallsCount++;
		}

		public SingletonPropertyDependency Dependency { get; set; }
	}
}