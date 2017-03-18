namespace Castle.MicroKernel.Tests.Configuration.Components
{
    using Castle.MicroKernel.SubSystems.Conversion;

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
