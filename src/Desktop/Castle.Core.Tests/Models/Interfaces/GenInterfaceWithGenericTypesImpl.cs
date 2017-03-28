using System.Collections;
using System.Collections.Generic;

namespace Castle.Core.Tests.GenInterfaces
{
	public class GenInterfaceWithGenericTypesImpl : GenInterfaceWithGenericTypes
	{
		public IList Find(string[,] query)
		{
			return new string[0];
		}

		public IList<T> Find<T>(string query)
		{
			return new List<T>();
		}

		public IList<string> FindStrings(string query)
		{
			return new List<string>();
		}

		public void Populate<T>(IList<T> list)
		{
		}

		public IList Find(string query)
		{
			return new string[0];
		}
	}
}