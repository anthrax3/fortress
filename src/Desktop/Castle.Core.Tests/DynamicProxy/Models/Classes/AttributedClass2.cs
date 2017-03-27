using System.IO;

namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	[ComplexNonInheritable(1, 2, true, "class", FileAccess.Write)]
	public class AttributedClass2
	{
		[ComplexNonInheritable(2, 3, "Do1", Access = FileAccess.ReadWrite)]
		public virtual void Do1()
		{
		}

		[ComplexNonInheritable(3, 4, "Do2", IsSomething = true)]
		public virtual void Do2()
		{
		}
	}
}