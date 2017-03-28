using System;

namespace Castle.Core.Tests
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class AttributeWithEnumArrayArgument : Attribute
	{
		public AttributeWithEnumArrayArgument(params SomeByteEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeSbyteEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeShortEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeUshortEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeIntEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeUintEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeLongEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}

		public AttributeWithEnumArrayArgument(params SomeUlongEnumForAttributeWithEnumArrayArgument[] attributeEnums)
		{
		}
	}
}