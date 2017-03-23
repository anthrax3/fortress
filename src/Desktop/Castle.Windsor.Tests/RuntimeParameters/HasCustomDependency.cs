namespace Castle.Windsor.Tests.RuntimeParameters
{
	public class HasCustomDependency
	{
		private CompA name;

		public HasCustomDependency(CompA name)
		{
			this.name = name;
		}
	}
}