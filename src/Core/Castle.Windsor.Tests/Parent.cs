using System.Collections.Generic;
using Castle.Windsor.MicroKernel;

namespace Castle.Windsor.Tests
{
	public class Parent : List<IChild>, IParent
	{
		public Parent(IKernel kernel)
		{
		}
	}
}