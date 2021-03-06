// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Reflection;
using Castle.Core.Tests.BugsReported;
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.InterClasses;
using Castle.Core.Tests.Interfaces;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Xunit;

namespace Castle.Core.Tests
{
	public class BasicInterfaceProxyTestCase : CoreBaseTestCase
	{
		private ParameterInfo[] GetMyTestMethodParams(Type type)
		{
			var methodInfo = type.GetTypeInfo().GetMethod("MyTestMethod", BindingFlags.Instance | BindingFlags.Public);
			return methodInfo.GetParameters();
		}

		private interface PrivateInterface
		{
		}

		[Fact]
		public void BaseTypeForInterfaceProxyHonored()
		{
			var options = new ProxyGenerationOptions();
			options.BaseTypeForInterfaceProxy = typeof(SimpleClass);
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), Type.EmptyTypes, options);

			Assert.IsAssignableFrom<SimpleClass>(proxy);
		}

		[Fact]
		public void BasicInterfaceProxyWithValidTarget()
		{
			var logger = new LogInvocationInterceptor();

			var service = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), logger);

			Assert.Equal(3, service.Sum(1, 2));

			Assert.Equal("Sum ", logger.LogContents);
		}

		[Fact]
		public void BasicInterfaceProxyWithValidTarget2()
		{
			var logger = new LogInvocationInterceptor();

			var service = (IService2)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService2), new Service2(), logger);

			service.DoOperation2();

			Assert.Equal("DoOperation2 ", logger.LogContents);
		}

		[Fact]
		public void Caching()
		{
#pragma warning disable 219
			var service = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), new StandardInterceptor());
			service = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), new StandardInterceptor());
#pragma warning restore 219
		}

		[Fact]
		public void Can_proxy_generic_interface()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IList<object>));
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_inaccessible_type_argument()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IList<PrivateInterface>)));
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_type_argument_that_has_inaccessible_type_argument()
		{
			var expected = string.Format("Can not create proxy for type {0} because type {1} is not accessible. Make it public, or internal",
				typeof(IList<IList<PrivateInterface>>).FullName, typeof(PrivateInterface).FullName);

			var exception = Assert.Throws<GeneratorException>(() => generator.CreateInterfaceProxyWithoutTarget(typeof(IList<IList<PrivateInterface>>)));
		}

		[Fact]
		public void Cannot_proxy_generic_type_with_open_generic_type_parameter()
		{
			var innerType = typeof(IList<>);
			var targetType = innerType.MakeGenericType(typeof(IList<>));
			Assert.Throws<GeneratorException>(() => generator.CreateInterfaceProxyWithoutTarget(targetType));
		}

		[Fact]
		public void Cannot_proxy_inaccessible_interface()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(PrivateInterface)));
		}

		[Fact]
		public void Cannot_proxy_open_generic_type()
		{
			var ex = Assert.Throws<GeneratorException>(() => generator.CreateInterfaceProxyWithoutTarget(typeof(IList<>)));
		}

		[Fact]
		public void CantCreateInterfaceTargetedProxyWithoutInterface()
		{
			Assert.Throws<ArgumentException>(delegate
			{
#pragma warning disable 219
				var service = (IService2) generator.CreateInterfaceProxyWithTargetInterface(typeof(Service2), new Service2());
#pragma warning restore 219
			});
		}

		[Fact]
		public void ChangingInvocationTargetSucceeds()
		{
			var logger = new LogInvocationInterceptor();

			var service = (IService)
				generator.CreateInterfaceProxyWithTargetInterface(
					typeof(IService), new AlwaysThrowsServiceImpl(), new ChangeTargetInterceptor(new ServiceImpl()),
					logger);

			Assert.Equal(20, service.Sum(10, 10));
		}

		[Fact]
		public void Indexer()
		{
			var logger = new LogInvocationInterceptor();

			var service = (InterfaceWithIndexer)
				generator.CreateInterfaceProxyWithTarget(
					typeof(InterfaceWithIndexer), new ClassWithIndexer(), logger);

			Assert.Equal(1, service[1]);

			Assert.Equal("get_Item ", logger.LogContents);
		}

		[Fact]
		public void InterfaceInheritance()
		{
			var logger = new LogInvocationInterceptor();

			IService service = (IExtendedService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IExtendedService), new ServiceImpl(), logger);

			Assert.Equal(3, service.Sum(1, 2));

			Assert.Equal("Sum ", logger.LogContents);
		}

		[Fact]
		public void InterfaceTargetTypeProducesInvocationsThatCanChangeTarget()
		{
			var logger = new LogInvocationInterceptor();
			var invocationChecker = new AssertCanChangeTargetInterceptor();

			var service = (IService2)
				generator.CreateInterfaceProxyWithTargetInterface(
					typeof(IService2), new Service2(), invocationChecker, logger);

			service.DoOperation2();

			Assert.Equal("DoOperation2 ", logger.LogContents);
		}

		[Fact]
		public void MethodParamNamesAreReplicated()
		{
			// Try with interface proxy (with target)
			var i = generator.CreateInterfaceProxyWithTarget(typeof(IMyInterface), new MyClass(),
				new StandardInterceptor()) as IMyInterface;

			var methodParams = GetMyTestMethodParams(i.GetType());
			Assert.Equal("myParam", methodParams[0].Name);
		}

		[Fact]
		public void Should_implement_explicitly_duplicate_interface_members()
		{
			var type =
				generator.CreateInterfaceProxyWithoutTarget(typeof(IIdenticalOne), new[] {typeof(IIdenticalTwo)}).GetType();
			var method = type.GetTypeInfo().GetMethod("Foo", BindingFlags.Instance | BindingFlags.Public);
			Assert.NotNull(method);
			Assert.Same(method, type.GetTypeInfo().GetRuntimeInterfaceMap(typeof(IIdenticalOne)).TargetMethods[0]);
			var method2 = type.GetTypeInfo().GetMethod("IIdenticalTwo.Foo", BindingFlags.Instance | BindingFlags.Public);
			Assert.NotNull(method2);
		}

		[Fact]
		public void Should_properly_implement_two_interfaces_with_methods_with_identical_signatures()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IIdenticalOne), new[] {typeof(IIdenticalTwo)},
				new DoNothingInterceptor());
			(proxy as IIdenticalOne).Foo();
			(proxy as IIdenticalTwo).Foo();
		}

		[Fact]
		public void Should_properly_proxy_class_that_implements_interface_virtually_interceptable()
		{
			var proxy = generator.CreateClassProxy(typeof(IdenticalOneVirtual), new[] {typeof(IIdenticalOne)},
				ProxyGenerationOptions.Default);
			(proxy as IIdenticalOne).Foo();
		}

		[Fact]
		public void Should_properly_proxy_class_that_implements_interface_virtually_non_interceptable()
		{
			var proxy = generator.CreateClassProxy(typeof(IdenticalOneVirtual));
			(proxy as IIdenticalOne).Foo();
		}
	}
}