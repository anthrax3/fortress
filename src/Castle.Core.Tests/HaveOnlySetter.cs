using System;

namespace Castle.Core.Tests
{
	public class HaveOnlySetter : IHaveOnlySetter
	{
		public string Foo
		{
			set { throw new Exception("The method or operation is not implemented."); }
		}
	}
}