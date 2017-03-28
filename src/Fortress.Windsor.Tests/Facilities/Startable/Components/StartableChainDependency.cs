using Castle.Core;

namespace Castle.Windsor.Tests.Facilities.Startable.Components
{
	public class StartableChainDependency : IStartable
	{
		public static int createcount;
		public static int startcount;

		public StartableChainDependency(StartableChainGeneric<string> item)
		{
			++createcount;
		}

		public void Start()
		{
			++startcount;
		}

		public void Stop()
		{
		}
	}
}