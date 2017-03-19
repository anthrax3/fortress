using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.Tests.Config.Components
{
	[Convertible]
	public class Server
	{
		public string Ip { get; set; }

		public string Name { get; set; }
	}
}