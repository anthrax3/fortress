using System;

namespace Castle.Core.Tests
{
	public interface IHasEvent
	{
		event EventHandler MyEvent;
	}
}