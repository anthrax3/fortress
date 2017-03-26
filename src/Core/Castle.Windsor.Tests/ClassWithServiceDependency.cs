using Castle.Windsor.Tests.Components;

namespace Castle.Windsor.Tests
{
	public class ClassWithServiceDependency
	{
		private IService dependency;

		public ClassWithServiceDependency(IService dependency)
		{
			this.dependency = dependency;
		}
	}
}