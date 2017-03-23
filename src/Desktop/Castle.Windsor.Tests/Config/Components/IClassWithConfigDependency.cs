namespace Castle.Windsor.Tests.Config.Components
{
	public interface IClassWithConfigDependency
	{
		string GetName();

		string GetServerIp(string name);
	}
}