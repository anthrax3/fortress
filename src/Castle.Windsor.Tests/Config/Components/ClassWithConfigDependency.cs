using System.Linq;

namespace Castle.Windsor.Tests.Config.Components
{
	public class ClassWithConfigDependency : IClassWithConfigDependency
	{
		private readonly IConfig _config;

		public ClassWithConfigDependency(IConfig config)
		{
			_config = config;
		}

		public string GetName()
		{
			return _config.Name;
		}

		public string GetServerIp(string name)
		{
			return _config.Servers.First(s => s.Name == name).Ip;
		}
	}
}