using Castle.Core.Core.Configuration;
using Castle.Windsor.MicroKernel;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Config
{
	public class DummyFacility : IFacility
	{
		public void Terminate()
		{
		}

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			Assert.IsNotNull(facilityConfig);
			var childItem = facilityConfig.Children["item"];
			Assert.IsNotNull(childItem);
			Assert.AreEqual("value", childItem.Value);
		}
	}
}