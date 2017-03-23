using System.Collections.Generic;

namespace Castle.Windsor.Tests
{
	public abstract class BaseDevice : IDevice
	{
		public abstract IEnumerable<IDevice> Children { get; }

		public MessageChannel Channel { get; set; }
	}
}