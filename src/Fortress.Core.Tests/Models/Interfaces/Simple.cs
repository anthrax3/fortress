namespace Castle.Core.Tests.Interfaces
{
	public class Simple : ISimple
	{
		public int Count { get; private set; }

		public void Method()
		{
			Count++;
		}
	}
}