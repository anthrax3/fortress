using System;

namespace Castle.Core.Tests
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class AttributeWithIntArrayArgument : Attribute
	{
		public AttributeWithIntArrayArgument(params int[] ints)
		{
		}
	}
}