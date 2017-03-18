
// GVDM: TODO: Why are we abstracting logging frameworks? Why is this coming from castle.core? Why do we want these dependencies? What is going on? This is mental! 

//namespace CastleTests.LoggingFacility
//{
//	using Castle.Core.Logging;
//	using Castle.Core.Resource;
//	using Castle.Facilities.Logging;
//	using Castle.Services.Logging.Log4netIntegration;
//	using Castle.Windsor;
//	using Castle.Windsor.Installer;

//	using CastleTests.LoggingFacility.Tests.Classes;

//	using NUnit.Framework;

//	[TestFixture]
//	public class CustomFacilityTests
//	{
//		[Test]
//		public void ReadCustomFacilityConfigFromXML()
//		{
//			using (var container = new WindsorContainer())
//			{
//				container.Install(
//					Configuration.FromXml(
//						new StaticContentResource(
//							string.Format(
//								@"<castle>
//<facilities>
//<facility 
//  id='loggingfacility'
//  loggingApi='custom'
//  customLoggerFactory='{0}'
//  type='{1}'/>
//</facilities>
//</castle>",
//								typeof(CustomLog4NetFactory).AssemblyQualifiedName,
//								typeof(LoggingFacility).AssemblyQualifiedName))));
//				var logger = container.Resolve<ILogger>();
//				Assert.IsInstanceOf<Log4netLogger>(logger);
//			}
//		}
//	}
//}
