using Castle.Windsor.Core;

namespace Castle.Windsor.Tests
{
	[Transient]
	public class DN : IN
	{
		private ISP sp;
		private IWM vm;

		public DN(IWM vm, ISP sp)
		{
			this.vm = vm;
			this.sp = sp;
			CS = new BS();
		}

		public IS CS { get; }
	}
}