namespace Castle.Core.Tests.BugsReported
{
	public interface ICamera
	{
		int Id { get; }
		string Name { get; set; }
		string IPNumber { get; set; }
	}
}