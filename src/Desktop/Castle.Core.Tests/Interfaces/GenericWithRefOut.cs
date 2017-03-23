using System;

namespace Castle.Core.Tests.Interfaces
{
	[Serializable]
	public class GenericWithRefOut : IGenericWithRefOut
	{
		public void Do<T>(out T i)
		{
			i = default(T);
		}

		public void Did<T>(ref T i)
		{
			i = default(T);
		}
	}
}