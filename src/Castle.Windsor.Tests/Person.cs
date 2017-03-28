namespace Castle.Windsor.Tests
{
	public class Person
	{
		public IWatcher Watcher;

		public Person(IWatcher watcher)
		{
			Watcher = watcher;
		}
	}
}