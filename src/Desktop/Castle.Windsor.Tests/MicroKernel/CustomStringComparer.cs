using Castle.Windsor.MicroKernel.Context;

namespace Castle.Windsor.Tests.MicroKernel
{
	public class CustomStringComparer : IArgumentsComparer
	{
		public bool RunEqualityComparison(object x, object y, out bool areEqual)
		{
			if (x is string)
			{
				areEqual = true;
				return true;
			}
			areEqual = false;
			return false;
		}

		public bool RunHasCodeCalculation(object o, out int hashCode)
		{
			if (o is string)
			{
				hashCode = "boo!".GetHashCode();
				return true;
			}
			hashCode = 0;
			return false;
		}
	}
}