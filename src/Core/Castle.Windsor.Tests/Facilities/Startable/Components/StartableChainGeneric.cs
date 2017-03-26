namespace Castle.Windsor.Tests.Facilities.Startable.Components
{
	public class StartableChainGeneric<T>
	{
		public static int createcount;

		public StartableChainGeneric()
		{
			++createcount;
		}
	}
}