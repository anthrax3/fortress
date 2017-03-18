
// GVDM: TODO: Why are we abstracting logging frameworks? Why is this coming from castle.core? Why do we want these dependencies? What is going on? This is mental! 


//namespace CastleTests.LoggingFacility.Tests.Classes
//{
//	using System;

//	using Castle.Core.Logging;
//	using Castle.Services.Logging.Log4netIntegration;

//	using log4net;
//	using log4net.Config;

//	public class CustomLog4NetFactory : Log4netFactory
//	{
//		public CustomLog4NetFactory()
//		{
//			BasicConfigurator.Configure();
//		}

//		public override ILogger Create(String name)
//		{
//			var log = LogManager.GetLogger(name);
//			return new Log4netLogger(log.Logger, this);
//		}

//		public override ILogger Create(String name, LoggerLevel level)
//		{
//			throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
//		}
//	}
//}

