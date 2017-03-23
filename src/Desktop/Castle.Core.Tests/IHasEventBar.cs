using System;

namespace Castle.Core.Tests
{
	public interface IHasEventBar : IHasEvent
	{
		event EventHandler Bar;
	}
}