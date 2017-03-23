using System;

namespace Castle.Core.Tests
{
	[Serializable]
	public class GenericClass : IGenericInterface
	{
		#region IGenericInterface Members

		public T GenericMethod<T>()
		{
			return default(T);
		}

		#endregion
	}
}