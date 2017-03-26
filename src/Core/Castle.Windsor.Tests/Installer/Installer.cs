using System;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Windsor;

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