using System;
using System.Reflection;
using Castle.Core.DynamicProxy;

namespace Castle.Core.Tests
{
	public class ISimpleInterface_Do
	{
		public ISimpleInterface_Do(SimpleClass simpleClass, FakeProxy fakeProxy, IInterceptor[] interceptorsDo, MethodInfo tokenDo, object[] objArray)
		{
			throw new NotImplementedException();
		}

		public object ReturnValue
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Proceed()
		{
			throw new NotImplementedException();
		}
	}
}