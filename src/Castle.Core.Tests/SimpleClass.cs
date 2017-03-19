using System;

namespace Castle.Core.Tests
{
	[Serializable]
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