// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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
using System.Collections.Generic;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.GenClasses;
using Castle.Core.Tests.Interceptors;
using Castle.DynamicProxy.Generators;
using Xunit;


namespace Castle.Core.Tests
{
	public class GenericClassProxyTestCase : CoreBaseTestCase
	{
		public  GenericClassProxyTestCase() : base()
		{
			logger = new LogInvocationInterceptor();
		}

		private LogInvocationInterceptor logger;

		[Fact]
		public void ClassWithGenMethodOnly()
		{
			var proxy = generator.CreateClassProxy<OnlyGenMethodsClass>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething(new List<object>());

			Assert.True(proxy.Invoked);
			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void GenericMethodArgumentsAndTypeGenericArgumentsWithSameName()
		{
			var proxy = generator.CreateClassProxy<GenClassNameClash<List<object>, List<object>>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomethingT(1);
			proxy.DoSomethingZ(1L);
			proxy.DoSomethingTX(1, "a");
			proxy.DoSomethingZX(1L, "b");

			Assert.Equal("DoSomethingT DoSomethingZ DoSomethingTX DoSomethingZX ", logger.LogContents);
		}

		[Fact]
		public void GenericProxyWithIndexer()
		{
			object proxy = generator.CreateClassProxy<ClassWithIndexer<string, int>>(logger);

			Assert.NotNull(proxy);

			var type = (ClassWithIndexer<string, int>) proxy;

			type["name"] = 10;
			Assert.Equal(10, type["name"]);

			Assert.Equal("set_Item get_Item ", logger.LogContents);
		}

		[Fact]
		public void MethodInfoClosedInGenTypeGenMethodRefType()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<GenClassWithGenMethods<List<object>>>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>),
				typeof(int));

			proxy.DoSomething(new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>),
				typeof(List<object>));
		}

		[Fact]
		public void MethodInfoClosedInGenTypeGenMethodValueType()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<GenClassWithGenMethods<int>>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));

			proxy.DoSomething(new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int),
				typeof(List<object>));
		}

		[Fact]
		public void MethodInfoClosedInGenTypeNongenMethodRefTypeRefType()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<GenClassWithGenReturn<List<object>, List<object>>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>));

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>));
		}

		[Fact]
		public void MethodInfoClosedInGenTypeNongenMethodValueTypeRefType()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<GenClassWithGenReturn<int, List<object>>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			Assert.Equal(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget().GetBaseDefinition());

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>));
			Assert.Equal(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget().GetBaseDefinition());
		}

		[Fact]
		public void MethodInfoClosedInGenTypeNongenMethodValueTypeValueType()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<GenClassWithGenReturn<int, int>>(interceptor);

			proxy.DoSomethingT();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			Assert.Equal(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget().GetBaseDefinition());

			proxy.DoSomethingZ();
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int));
			Assert.Equal(interceptor.Invocation.GetConcreteMethod(),
				interceptor.Invocation.GetConcreteMethodInvocationTarget().GetBaseDefinition());
		}

		[Fact]
		public void MethodInfoClosedInNongenTypeGenMethod()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateClassProxy<OnlyGenMethodsClass>(interceptor);

			proxy.DoSomething(1);
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(int), typeof(int));

			proxy.DoSomething(new List<object>());
			GenericTestUtility.CheckMethodInfoIsClosed(interceptor.Invocation.GetConcreteMethod(), typeof(List<object>),
				typeof(List<object>));
		}

		[Fact]
		public void ProxyWithGenericArgument()
		{
			var proxy = generator.CreateClassProxy<ClassWithGenArgs<int>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething();

			Assert.True(proxy.Invoked);

			proxy.AProperty = true;
			Assert.True(proxy.AProperty);

			Assert.Equal("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArguments()
		{
			var proxy = generator.CreateClassProxy<ClassWithGenArgs<int, string>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething();

			Assert.True(proxy.Invoked);

			proxy.AProperty = true;
			Assert.True(proxy.AProperty);

			Assert.Equal("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsAndArgumentConstraints()
		{
			var proxy = generator.CreateClassProxy<GenClassWithConstraints<int>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething();

			Assert.True(proxy.Invoked);

			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsAndMethodGenericArguments()
		{
			var proxy = generator.CreateClassProxy<GenClassWithGenMethods<List<object>>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething("z param");

			Assert.True(proxy.Invoked);
			Assert.Equal("z param", proxy.SavedParam);
			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsAndMethodGenericArgumentsWithConstraints()
		{
			var proxy = generator.CreateClassProxy<GenClassWithGenMethodsConstrained<List<object>>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething("z param");

			Assert.True(proxy.Invoked);
			Assert.Equal("z param", proxy.SavedParam);
			Assert.Equal("DoSomething ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsAndMethodGenericArgumentsWithOneNotDefinedOnType()
		{
			var proxy = generator.CreateClassProxy<GenClassWithGenMethods<List<object>>>(logger);

			Assert.NotNull(proxy);

			var value1 = 10;

			proxy.DoSomethingElse(delegate(int param1) { return param1.ToString(); }, value1);

			Assert.True(proxy.Invoked);
			Assert.Equal("10", proxy.SavedParam);
			Assert.Equal("DoSomethingElse ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsAndMethodGenericReturn()
		{
			var proxy = generator.CreateClassProxy<GenClassWithGenReturn<List<object>, List<object>>>(logger);

			Assert.NotNull(proxy);

			object ret1 = proxy.DoSomethingT();
			object ret2 = proxy.DoSomethingZ();

			Assert.IsType<List<object>>(ret1);
			Assert.IsType<List<object>>(ret2);
			Assert.Equal("DoSomethingT DoSomethingZ ", logger.LogContents);
		}

		[Fact]
		public void ProxyWithGenericArgumentsWithBaseGenericClass()
		{
			var proxy = generator.CreateClassProxy<SubClassWithGenArgs<int, string, int>>(logger);

			Assert.NotNull(proxy);

			proxy.DoSomething();

			Assert.True(proxy.Invoked);

			proxy.AProperty = true;
			Assert.True(proxy.AProperty);

			Assert.Equal("DoSomething set_AProperty get_AProperty ", logger.LogContents);
		}

		[Fact]
		public void ThrowsWhenProxyingGenericTypeDefNoTarget()
		{
			var interceptor = new KeepDataInterceptor();

			Assert.Throws<GeneratorException>(delegate { generator.CreateClassProxy(typeof(GenClassWithGenReturn<,>), interceptor); });
		}

		[Fact]
		public void TypeWithGenericMethodHavingArgumentBeingGenericArrayOfT()
		{
			var proxy = generator.CreateClassProxy<MethodWithArgumentBeingArrayOfGenericTypeOfT>();
			Assert.NotNull(proxy);
			proxy.Method(new Action<string>[0]);
		}
	}
}