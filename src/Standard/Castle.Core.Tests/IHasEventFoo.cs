using System;

namespace Castle.Core.Tests
{
	public interface IHasEventFoo : IHasEvent
	{
		event EventHandler EventFoo;
	}
}