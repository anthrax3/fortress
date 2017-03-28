using System;

namespace Castle.Core.Tests
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class AttributeWithTypeArrayArgument : Attribute
	{
		public AttributeWithTypeArrayArgument(params Type[] attributeTypes)
		{
		}
	}
}