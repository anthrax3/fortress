using System.Collections.Generic;

namespace Castle.Windsor.Tests
{
	public class MyObject : IMyObject
	{
		protected readonly IDictionary<int, IList<string>> stuff;

		public MyObject(IDictionary<int, IList<string>> stuff)
		{
			this.stuff = stuff;
		}

		public virtual int Count
		{
			get { return stuff.Count; }
		}
	}
}