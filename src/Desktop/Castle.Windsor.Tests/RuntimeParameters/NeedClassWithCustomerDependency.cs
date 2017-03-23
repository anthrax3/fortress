namespace Castle.Windsor.Tests.RuntimeParameters
{
	public class NeedClassWithCustomerDependency
	{
		private HasCustomDependency dependency;

		public NeedClassWithCustomerDependency(HasCustomDependency dependency)
		{
			this.dependency = dependency;
		}
	}
}