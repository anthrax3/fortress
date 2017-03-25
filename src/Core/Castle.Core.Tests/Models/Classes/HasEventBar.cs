using System;

namespace Castle.Core.Tests
{
	public class HasEventBar : IHasEventBar
	{
		public void RaiseMyEvent()
		{
			MyEvent(null, EventArgs.Empty);
		}

		public void RaiseBar()
		{
			Bar(null, EventArgs.Empty);
		}

		#region IHasEventBar Members

		public event EventHandler MyEvent;
		public event EventHandler Bar;

		#endregion
	}
}