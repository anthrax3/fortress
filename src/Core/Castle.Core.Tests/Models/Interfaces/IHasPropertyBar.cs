namespace Castle.Core.Tests
{
	public interface IHasPropertyBar : IHasProperty
	{
		string Bar { get; set; }
	}
}