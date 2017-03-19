using Castle.Windsor.Core;

namespace Castle.Windsor.MicroKernel
{
	public delegate void DependencyDelegate(ComponentModel client, DependencyModel model, object dependency);
}