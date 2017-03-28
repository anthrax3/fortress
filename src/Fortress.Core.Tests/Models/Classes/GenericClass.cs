using System;

namespace Castle.Core.Tests
{
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