namespace Castle.MicroKernel.Tests.Configuration.Components
{
    using System.Linq;

    public interface IClassWithConfigDependency
    {
        string GetName();

        string GetServerIp(string name);
    }

    public class ClassWithConfigDependency : IClassWithConfigDependency
    {
        private readonly IConfig _config;

        public ClassWithConfigDependency(IConfig config)
        {
            this._config = config;
        }

        public string GetName()
        {
            return this._config.Name;
        }

        public string GetServerIp(string name)
        {
            return this._config.Servers.First(s => s.Name == name).Ip;
        }
    }
}
