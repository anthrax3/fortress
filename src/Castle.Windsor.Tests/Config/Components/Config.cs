using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.Tests.Config.Components
{
	public interface IConfig
    {
        string Name { get; set; }

        Server[] Servers { get; set; }
    }

    public class Config : IConfig
    {
        public string Name { get; set; }

        public Server[] Servers { get; set; }
    }

    [Convertible]
    public class Server
    {
        public string Ip { get; set; }

        public string Name { get; set; }
    }
}
