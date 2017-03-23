using Castle.Facilities.Logging;
using Castle.Windsor.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Windsor;

namespace Castle.Windsor.Tests.LoggingFacility
{
	public abstract class OverrideLoggerTest : BaseTest
	{
		protected virtual IWindsorContainer CreateConfiguredContainer(LoggerImplementation loggerApi, string custom, string logName)
		{
			IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());
			var configFile = GetConfigFile(loggerApi);

			container.AddFacility<Castle.Facilities.Logging.LoggingFacility>(f => f.LogUsing(loggerApi).WithConfig(configFile).ToLog(logName));

			return container;
		}
	}
}