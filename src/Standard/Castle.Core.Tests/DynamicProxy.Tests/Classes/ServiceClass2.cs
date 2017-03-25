namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	public class ServiceClass2
	{
		public void DoSomething()
		{
			DoOtherThing();
		}

		public virtual void DoOtherThing()
		{
			DoSomethingElse();
		}

		public virtual void DoSomethingElse()
		{
		}
	}
}