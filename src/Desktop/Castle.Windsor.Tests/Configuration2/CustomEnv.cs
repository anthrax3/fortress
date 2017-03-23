using Castle.Windsor.Windsor;

namespace Castle.Windsor.Tests.Configuration2
{
	internal class CustomEnv : IEnvironmentInfo
	{
		private readonly bool isDevelopment;

		public CustomEnv(bool isDevelopment)
		{
			this.isDevelopment = isDevelopment;
		}

		public string GetEnvironmentName()
		{
			return isDevelopment ? "devel" : "test";
		}
	}
}