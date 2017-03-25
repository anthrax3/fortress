namespace Castle.Core.Tests
{
	public class MyServiceImpl : IMyService
	{
		public ISomething CreateSomething<T>(string somethingSpec)
		{
			return new NoOpSomething();
		}

		public ISomething CreateSomething(string somethingKey)
		{
			return new NoOpSomething();
		}
	}
}