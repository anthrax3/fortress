using System;

namespace Castle.Core.Tests.InterClasses
{
	[Serializable]
	[My("MyInterfaceImpl")]
	public class MyInterfaceImpl : IMyInterface2
	{
		private string _name;
		private bool _started;

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual bool Started
		{
			get { return _started; }
			set { _started = value; }
		}

		[My("Calc1")]
		public virtual int Calc(int x, int y)
		{
			return x + y;
		}

		[My("Calc2")]
		public virtual int Calc(int x, int y, int z, float k)
		{
			return x + y + z + (int) k;
		}
	}
}