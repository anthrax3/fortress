using System.Collections.Generic;

namespace Castle.Core.Tests.GenInterfaces
{
	public interface InterfaceWithExplicitImpl<T>
	{
		IEnumerator<T> GetEnum1();
	}
}