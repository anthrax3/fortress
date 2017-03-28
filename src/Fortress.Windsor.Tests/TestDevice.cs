using System.Collections.Generic;

namespace Castle.Windsor.Tests
{
	public class TestDevice : BaseDevice
	{
		private readonly List<IDevice> children;

		public TestDevice()
		{
		}

		public TestDevice(IEnumerable<IDevice> theChildren)
		{
			children = new List<IDevice>(theChildren);
		}

		public override IEnumerable<IDevice> Children
		{
			get { return children; }
		}
	}
}