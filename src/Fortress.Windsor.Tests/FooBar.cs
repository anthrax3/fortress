using System;
using Castle.Core;

namespace Castle.Windsor.Tests
{
	[Transient]
	public class FooBar
	{
		public FooBar(int integer)
		{
		}

		public FooBar(DateTime datetime)
		{
		}
	}
}