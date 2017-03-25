namespace Castle.Core.Tests.DynamicProxy.Tests.Classes
{
	public class ExtendedServiceClass : ServiceClass
	{
		public virtual bool Valid2
		{
			get { return false; }
		}

		public virtual ulong Sum2(ulong b1, ulong b2)
		{
			return b1 + b2;
		}
	}
}