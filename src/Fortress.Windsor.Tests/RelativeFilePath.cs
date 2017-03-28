namespace Castle.Windsor.Tests.IOC51
{
	public class RelativeFilePath : IPathProvider
	{
		public RelativeFilePath(IPathProvider basePathProvider, string extensionsPath)
		{
			Path = System.IO.Path.Combine(basePathProvider.Path + "\\", extensionsPath);
		}

		public string Path { get; }
	}
}