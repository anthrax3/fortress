namespace Castle.Windsor.Tests
{
	public class ClassWithPrimitiveDependency
	{
		private int dependency;

		public ClassWithPrimitiveDependency(int dependency)
		{
			this.dependency = dependency;
		}
	}
}