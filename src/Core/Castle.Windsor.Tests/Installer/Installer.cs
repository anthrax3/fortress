using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace Castle.Windsor.Tests.Installer
{
	internal class Installer : IWindsorInstaller
	{
		private readonly Action<IWindsorContainer> install;

		public Installer(Action<IWindsorContainer> install)
		{
			this.install = install;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			install(container);
		}
	}
}