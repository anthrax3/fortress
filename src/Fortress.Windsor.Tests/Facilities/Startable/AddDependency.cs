using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Castle.Windsor.Tests.Facilities.Startable
{
	public class AddDependency : IComponentModelDescriptor
	{
		private readonly DependencyModel dependency;

		public AddDependency(DependencyModel dependency)
		{
			this.dependency = dependency;
		}

		public void BuildComponentModel(IKernel kernel, ComponentModel model)
		{
			model.Dependencies.Add(dependency);
		}

		public void ConfigureComponentModel(IKernel kernel, ComponentModel model)
		{
		}
	}
}