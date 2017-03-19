namespace Castle.Windsor.Tests.Config.Components
{
	public interface IConfig
	{
		string Name { get; set; }

		Server[] Servers { get; set; }
	}
}