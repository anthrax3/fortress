namespace Castle.Core.Tests
{
	public interface IMyService
	{
		ISomething CreateSomething<T>(string somethingSpec);
		ISomething CreateSomething(string somethingKey);
	}
}