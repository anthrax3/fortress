using Castle.Windsor.Core;

namespace Castle.Windsor.Tests
{
	[Singleton]
	public class SingletonPropertyDependency
	{
		public SingletonPropertyComponent Component { get; set; }
	}
}