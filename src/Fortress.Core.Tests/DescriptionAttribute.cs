using System;

namespace Castle.Core.Tests
{
	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute(string description)
		{
			Description = description;
		}

		public string Description { get; set; }
	}
}