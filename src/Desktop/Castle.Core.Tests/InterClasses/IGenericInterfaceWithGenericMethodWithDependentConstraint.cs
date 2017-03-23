namespace Castle.Core.Tests.InterClasses
{
	public interface IGenericInterfaceWithGenericMethodWithDependentConstraint<TFrom>
	{
		IInterfaceWithGenericMethodWithDependentConstraint RegisterType<TTo>() where TTo : TFrom;
	}
}