using System;

namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SmartAttribute : Attribute
	{
		private int value = -1;

		public int Value
		{
			get { return value; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Got ya!");
				this.value = value;
			}
		}
	}
}