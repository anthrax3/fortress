namespace Castle.Windsor.MicroKernel.Registration
{
	public abstract class Child
	{
		public static NamedChild ForName(string name)
		{
			return new NamedChild(name);
		}
	}
}