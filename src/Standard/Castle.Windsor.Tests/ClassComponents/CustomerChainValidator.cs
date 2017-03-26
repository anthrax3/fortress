namespace Castle.Windsor.Tests.ClassComponents
{
	public class CustomerChainValidator<T> : IValidator<T>
		where T : CustomerChain1
	{
		public bool IsValid(T customerChain)
		{
			return true;
		}
	}
}