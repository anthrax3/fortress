using System;

namespace Castle.Core.Tests
{
	public class NoOpSomething : ISomething
	{
		public void Do(Type type, string parameter)
		{
		}
	}
}