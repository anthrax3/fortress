using System;

namespace Castle.Core.Tests
{
	public interface IHasPropertyFoo : IHasProperty
	{
		DateTime Foo { get; set; }
	}
}