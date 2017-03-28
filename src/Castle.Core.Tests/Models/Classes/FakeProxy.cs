using System;
using System.Reflection;
using System.Xml.Serialization;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Internal;

namespace Castle.Core.Tests
{
	public class FakeProxy
	{
		// Fields
		public static ProxyGenerationOptions proxyGenerationOptions;
		public static MethodInfo token_Do;

		public IInterceptor[] __interceptors;

		public IInterceptorSelector __selector;

		public SimpleClass __target;

		public IInterceptor[] interceptors_Do;

		public virtual int Do()
		{
			// This item is obfuscated and can not be translated.
			if (interceptors_Do == null)
				interceptors_Do = __selector.SelectInterceptors(TypeUtil.GetTypeOrNull(__target), token_Do, __interceptors) ?? new IInterceptor[0];
			var objArray = new object[0];
			var @do = new ISimpleInterface_Do(__target, this, interceptors_Do, token_Do, objArray);
			@do.Proceed();
			return (int) @do.ReturnValue;
		}
	}
}