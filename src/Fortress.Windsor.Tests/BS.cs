using Castle.Core;

namespace Castle.Windsor.Tests
{
	[Transient]
	public class BS : IS
	{
		public ISP SP { get; set; } = null;
	}
}