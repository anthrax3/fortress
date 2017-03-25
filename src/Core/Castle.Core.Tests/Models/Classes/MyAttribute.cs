using System;

namespace Castle.Core.Tests.InterClasses
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
	public class MyAttribute : Attribute
	{
		public MyAttribute(string name)
		{
			this.name = name;
		}

		public string name { get; }
	}
}