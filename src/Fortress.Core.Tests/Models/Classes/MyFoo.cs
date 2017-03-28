using System;

namespace Castle.Core.Tests.BugsReported
{
	public class MyFoo : Inherited, ISub1, ISub2
	{
		void ISub1.Bar()
		{
			throw new NotImplementedException();
		}

		void IBase.Foo()
		{
			throw new NotImplementedException();
		}

		public virtual void Baz()
		{
			throw new NotImplementedException();
		}
	}
}