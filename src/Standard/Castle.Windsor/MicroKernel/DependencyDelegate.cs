using Castle.Core;

namespace Castle.MicroKernel
{
	public delegate void DependencyDelegate(ComponentModel client, DependencyModel model, object dependency);
}