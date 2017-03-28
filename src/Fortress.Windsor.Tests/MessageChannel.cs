namespace Castle.Windsor.Tests
{
	public class MessageChannel
	{
		public MessageChannel(IDevice root)
		{
			RootDevice = root;
		}

		public IDevice RootDevice { get; }
	}
}