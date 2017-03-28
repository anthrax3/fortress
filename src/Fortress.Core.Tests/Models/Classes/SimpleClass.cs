using System;

namespace Castle.Core.Tests
{
	public class SimpleClass : ISimpleInterface
	{
		#region ISimpleInterface Members

		public int Do()
		{
			return 3;
		}

		#endregion
	}
}