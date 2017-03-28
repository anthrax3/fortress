namespace Castle.Core.Tests.GenClasses
{
	public interface IChangeTracking
	{
		bool IsChanged { get; }

		void AcceptChanges();
	}
}