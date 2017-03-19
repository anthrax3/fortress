namespace Castle.Windsor.Tests.Config.Components
{
	public class Config : IConfig
	{
		public string Name { get; set; }

		public Server[] Servers { get; set; }
	}
}