using System;
using System.Reflection;
using Castle.Core.DynamicProxy;

namespace Castle.Core.Tests
{
	public class SelectorWithState : IInterceptorSelector
	{
		private readonly int state;

		public SelectorWithState(int state)
		{
			this.state = state;
		}

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			return interceptors;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((SelectorWithState) obj);
		}

		public override int GetHashCode()
		{
			return state;
		}

		protected bool Equals(SelectorWithState other)
		{
			return state == other.state;
		}
	}
}