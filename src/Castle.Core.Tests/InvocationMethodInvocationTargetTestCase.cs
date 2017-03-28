// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.GenClasses;
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.InterClasses;
using Castle.Core.Tests.Interfaces;
using Xunit;


namespace Castle.Core.Tests
{
	public class InvocationMethodInvocationTargetTestCase : CoreBaseTestCase
	{
		[Fact]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<ServiceClass>(interceptor);
			proxy.Sum(2, 2);
			var methodOnClass = typeof(ServiceClass).GetTypeInfo().GetMethod("Sum", new[] {typeof(int), typeof(int)});
			Assert.Same(methodOnClass, interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method_for_interface_methods_implemented_non_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(One), new[] {typeof(IOne)}, interceptor) as IOne;
			proxy.OneMethod();
			var methodOnInterface = typeof(One).GetTypeInfo().GetMethod("OneMethod", Type.EmptyTypes);
			Assert.Same(methodOnInterface, interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void ClassProxy_MethodInvocationTarget_should_be_base_Method_for_interface_methods_implemented_virtually()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy(typeof(ClassWithVirtualInterface), new[] {typeof(ISimpleInterface)}, interceptor) as ISimpleInterface;
			proxy.Do();
			var methodOnClass = typeof(ClassWithVirtualInterface).GetTypeInfo().GetMethod("Do", Type.EmptyTypes);
			Assert.Same(methodOnClass, interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void ClassProxyForGeneric_MethodInvocationTarget_should_be_proxyMethod()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = (IChangeTracking) generator.CreateClassProxy<GenClassWithExplicitImpl<int>>(interceptor);
			Assert.True(proxy.IsChanged);
			Assert.NotNull(interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void InterfaceProxyWithTarget_MethodInvocationTarget_should_be_methodOnTargetType()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTarget<IService>(target, interceptor);
			proxy.Sum(2, 2);
			var methodOnTarget = target.GetType().GetTypeInfo().GetMethod("Sum", new[] {typeof(int), typeof(int)});
			Assert.Same(methodOnTarget, interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void InterfaceProxyWithTarget_MethodInvocationTarget_should_be_null()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IService>(interceptor);
			proxy.Sum(2, 2);
			Assert.Null(interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_MethodInvocationTarget_should_be_methodOnTargetType()
		{
			var interceptor = new KeepDataInterceptor();
			var target = new ServiceImpl();
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), target, interceptor) as IService;
			proxy.Sum(2, 2);
			var methodOnTarget = target.GetType().GetTypeInfo().GetMethod("Sum", new[] {typeof(int), typeof(int)});
			Assert.Same(methodOnTarget, interceptor.Invocation.MethodInvocationTarget);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_MethodInvocationTarget_should_be_updated_when_target_changes()
		{
			MethodInfo invocationTarget1 = null;
			MethodInfo invocationTarget2 = null;
			var target1 = new AlwaysThrowsServiceImpl();
			var target2 = new ServiceImpl();
			var methodOnTarget1 = target1.GetType().GetTypeInfo().GetMethod("Sum", new[] {typeof(int), typeof(int)});
			var methodOnTarget2 = target2.GetType().GetTypeInfo().GetMethod("Sum", new[] {typeof(int), typeof(int)});
			var proxy = generator.CreateInterfaceProxyWithTargetInterface(
				typeof(IService),
				target1,
				new WithCallbackInterceptor(i =>
				{
					invocationTarget1 = i.MethodInvocationTarget;
					i.Proceed();
				}),
				new ChangeTargetInterceptor(target2),
				new WithCallbackInterceptor(i =>
				{
					invocationTarget2 = i.MethodInvocationTarget;
					i.Proceed();
				})) as IService;

			proxy.Sum(2, 2);

			Assert.NotEqual(invocationTarget1, invocationTarget2);
			Assert.Same(methodOnTarget1, invocationTarget1);
			Assert.Same(methodOnTarget2, invocationTarget2);
		}
	}
}