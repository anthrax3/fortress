using Castle.Core.Tests.Interfaces;

namespace Castle.Core.Tests
{
	public class IdenticalOneVirtual : IIdenticalOne
	{
		public virtual string Foo()
		{
			return "Foo";
		}
	}
}