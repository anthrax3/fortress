using System.Collections.Generic;

namespace Castle.Windsor.Core.Internal
{
	internal class TimestampSet
	{
		private readonly IDictionary<IVertex, int> items = new Dictionary<IVertex, int>();

		public void Register(IVertex item, int time)
		{
			items[item] = time;
		}

		public int TimeOf(IVertex item)
		{
			return items[item];
		}
	}
}