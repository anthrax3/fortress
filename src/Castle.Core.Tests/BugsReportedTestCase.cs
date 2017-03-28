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
using Castle.Core.Tests.BugsReported;
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.InterClasses;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class BugsReportedTestCase : CoreBaseTestCase
	{
		[Fact]
		public void CallingProceedWithInterceptorOnAbstractMethodShouldThrowException()
		{
			var proxy = generator.CreateClassProxy<AbstractClass>(ProxyGenerationOptions.Default, new StandardInterceptor());
			Assert.NotNull(proxy);

			var ex = Assert.Throws(typeof(NotImplementedException), () => proxy.Foo());

			var message =
				"This is a DynamicProxy2 error: The interceptor attempted to 'Proceed' for method 'System.String Foo()' which is abstract. " +
				"When calling an abstract method there is no implementation to 'proceed' to " +
				"and it is the responsibility of the interceptor to mimic the implementation (set return value, out arguments etc)";
			Assert.Equal(message, ex.Message);
		}

		[Fact]
		public void CallingProceedWithoutInterceptorOnAbstractMethodShouldThrowException()
		{
			var proxy = generator.CreateClassProxy<AbstractClass>();
			Assert.NotNull(proxy);

			var ex = Assert.Throws<NotImplementedException>(() => proxy.Foo());

			var message =
				"This is a DynamicProxy2 error: There are no interceptors specified for method 'System.String Foo()' which is abstract. " +
				"When calling an abstract method there is no implementation to 'proceed' to " +
				"and it is the responsibility of the interceptor to mimic the implementation (set return value, out arguments etc)";
			Assert.Equal(message, ex.Message);
		}

		[Fact]
		public void DYNPROXY_51_GenericMarkerInterface()
		{
			var p =
				(WithMixin) generator.CreateClassProxy(typeof(WithMixin), new[] {typeof(Marker<int>)});
			p.Method();
		}

		[Fact]
		public void DYNPROXY_99_ClassProxyHasNamespace()
		{
			var type = generator.CreateClassProxy(typeof(ServiceImpl)).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithoutTargetHasNamespace()
		{
			var type = generator.CreateInterfaceProxyWithoutTarget(typeof(IService)).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithTargetHasNamespace()
		{
			var type = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl()).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void DYNPROXY_99_InterfaceProxyWithTargetInterfaceHasNamespace()
		{
			var type = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl()).GetType();
			Assert.NotNull(type.Namespace);
			Assert.Equal("Castle.Proxies", type.Namespace);
		}

		[Fact]
		public void InterfaceInheritance()
		{
			var proxy = (ICameraService)
				generator.CreateInterfaceProxyWithTarget(typeof(ICameraService),
					new CameraService(),
					new StandardInterceptor());

			Assert.NotNull(proxy);

			proxy.Add("", "");
			proxy.Record(null);
		}

		[Fact]
		public void ProxyInterfaceWithSetterOnly()
		{
			var proxy = (IHaveOnlySetter)
				generator.CreateInterfaceProxyWithTarget(typeof(IHaveOnlySetter),
					new HaveOnlySetter(),
					new DoNothingInterceptor());

			Assert.NotNull(proxy);

			proxy.Foo = "bar";
		}

		[Fact]
		public void ProxyTypeThatInheritFromGenericType()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IUserRepository>(new DoNothingInterceptor());
			Assert.NotNull(proxy);
		}
	}
}