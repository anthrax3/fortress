// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Handlers;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;
using Castle.Windsor.Tests.ClassComponents;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Tests.MicroKernel;
using Castle.Windsor.Windsor.Proxy;
using Xunit;

namespace Castle.Windsor.Tests
{
	
	public class MicroKernelTestCase : AbstractContainerTestCase
	{
		[Fact]
		public void AddClassComponentWithInterface()
		{
			Kernel.Register(Component.For<CustomerImpl>().Named("key"));
			Assert.True(Kernel.HasComponent("key"));
		}

		[Fact]
		public void AddClassComponentWithNoInterface()
		{
			Kernel.Register(Component.For(typeof(DefaultCustomer)).Named("key"));
			Assert.True(Kernel.HasComponent("key"));
		}

		[Fact]
		public void AddClassThatHasTwoParametersOfSameTypeAndNoOverloads()
		{
			Kernel.Register(Component.For(typeof(ClassWithTwoParametersWithSameType)).Named("test"));
			Kernel.Register(Component.For<ICommon>().ImplementedBy(typeof(CommonImpl1)).Named("test2"));
			var resolved = Kernel.Resolve(typeof(ClassWithTwoParametersWithSameType), new Dictionary<object, object>());
			Assert.NotNull(resolved);
		}

		[Fact]
		public void AddCommonComponent()
		{
			Kernel.Register(Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("key"));
			Assert.True(Kernel.HasComponent("key"));
		}

		[Fact]
		public void AddComponentInstance()
		{
			var customer = new CustomerImpl();
			Kernel.Register(Component.For<ICustomer>().Named("key").Instance(customer));
			Assert.True(Kernel.HasComponent("key"));

			var customer2 = Kernel.Resolve<CustomerImpl>("key");
			Assert.Same(customer, customer2);

			customer2 = Kernel.Resolve<ICustomer>() as CustomerImpl;
			Assert.Same(customer, customer2);
		}

		[Fact]
		public void AddComponentInstance_ByService()
		{
			var customer = new CustomerImpl();

			Kernel.Register(Component.For<ICustomer>().Instance(customer));
			Assert.Same(Kernel.Resolve<ICustomer>(), customer);
		}

		[Fact]
		public void AddComponentInstance2()
		{
			var customer = new CustomerImpl();

			Kernel.Register(Component.For<CustomerImpl>().Named("key").Instance(customer));
			Assert.True(Kernel.HasComponent("key"));

			var customer2 = Kernel.Resolve<CustomerImpl>("key");
			Assert.Same(customer, customer2);

			customer2 = Kernel.Resolve<CustomerImpl>();
			Assert.Same(customer, customer2);
		}

		[Fact]
		public void AdditionalParametersShouldNotBePropagatedInTheDependencyChain()
		{
			Kernel.Register(
				Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("cust").LifeStyle.Transient);
			Kernel.Register(Component.For<ExtendedCustomer>().Named("custex").LifeStyle.Transient);

			var dictionary = new Dictionary<string, object> {{"Name", "name"}, {"Address", "address"}, {"Age", "18"}};
			var customer = Kernel.Resolve<ICustomer>("cust", dictionary);

			Assert.Equal("name", customer.Name);
			Assert.Equal("address", customer.Address);
			Assert.Equal(18, customer.Age);

			var custImpl = customer as CustomerImpl;

			Assert.NotNull(custImpl.ExtendedCustomer);
			Assert.Null(custImpl.ExtendedCustomer.Address);
			Assert.Null(custImpl.ExtendedCustomer.Name);
			Assert.Equal(0, custImpl.ExtendedCustomer.Age);
		}

		[Fact]
		public void Can_use_custom_dependencyResolver()
		{
			var resolver = new NotImplementedDependencyResolver();
			var defaultKernel = new DefaultKernel(resolver, new DefaultProxyFactory());
			Assert.Same(resolver, defaultKernel.Resolver);
			Assert.Same(defaultKernel, resolver.Kernel);
		}

		[Fact]
		public void HandlerForClassComponent()
		{
			Kernel.Register(Component.For<CustomerImpl>().Named("key"));
			var handler = Kernel.GetHandler("key");
			Assert.NotNull(handler);
		}

		[Fact]
		public void HandlerForClassWithNoInterface()
		{
			Kernel.Register(Component.For<DefaultCustomer>().Named("key"));
			var handler = Kernel.GetHandler("key");
			Assert.NotNull(handler);
		}

		[Fact]
		public void IOC_50_AddTwoComponentWithSameService_RequestFirstByKey_RemoveFirst_RequestByService_ShouldReturnSecond()
		{
			Kernel.Register(Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("key"));
			Kernel.Register(Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("key2"));
			var result = Kernel.Resolve<object>("key");
			Assert.NotNull(result);

			result = Kernel.Resolve<ICustomer>();
			Assert.NotNull(result);
		}

		[Fact]
		public void KeyCollision()
		{
			Assert.Throws<ComponentRegistrationException>(() =>
			{
				Kernel.Register(Component.For<CustomerImpl>().Named("key"));
				Kernel.Register(Component.For<CustomerImpl>().Named("key"));
			});
		}

		[Fact]
		[Bug("IOC-327")]
		public void ReleaseComponent_null_silently_ignored_doesnt_throw()
		{
			Kernel.ReleaseComponent(null);
		}

		[Fact]
		public void Resolve_all_when_dependency_is_missing_throws_DependencyResolverException()
		{
			Kernel.Register(
				Component.For<C>());
			// the dependency goes C --> B --> A

			Assert.Throws<DependencyResolverException>(() => Kernel.ResolveAll<C>(new Arguments {{"fakeDependency", "Stefan!"}}));
		}

		[Fact]
		public void Resolve_all_when_dependency_is_unresolvable_throws_HandlerException()
		{
			Kernel.Register(
				Component.For<B>(),
				Component.For<C>());
			// the dependency goes C --> B --> A

			Assert.Throws<HandlerException>(() => Kernel.ResolveAll<C>());
		}

		[Fact]
		public void ResolveAll()
		{
			Kernel.Register(Component.For<ICommon>().ImplementedBy<CommonImpl2>());
			Kernel.Register(Component.For<ICommon>().ImplementedBy<CommonImpl1>());
			var services = Kernel.ResolveAll<ICommon>();
			Assert.Equal(2, services.Length);
		}

		[Fact]
		public void ResolveAll_does_handle_multi_service_components()
		{
			Kernel.Register(Component.For<ICommon>().ImplementedBy<CommonImpl2>().Named("test"));
			Kernel.Register(Component.For<ICommonSub1, ICommon>().ImplementedBy<CommonSub1Impl>().Named("test2"));
			var services = Kernel.ResolveAll<ICommon>();
			Assert.Equal(2, services.Length);
		}

		[Fact]
		public void ResolveAll_does_NOT_account_for_assignable_services()
		{
			Kernel.Register(Component.For<ICommon>().ImplementedBy<CommonImpl2>().Named("test"));
			Kernel.Register(Component.For<ICommonSub1>().ImplementedBy<CommonSub1Impl>().Named("test2"));
			var services = Kernel.ResolveAll<ICommon>();
			Assert.Equal(1, services.Length);
		}

		[Fact]
		public void ResolveAll_resolves_when_dependency_provideded_dynamically()
		{
			Kernel.Register(Component.For<ICommon>()
				.ImplementedBy<CommonImplWithDependency>()
				.DynamicParameters((k, d) => d.Insert(typeof(ICustomer), new CustomerImpl()))
			);

			var services = Kernel.ResolveAll<ICommon>();
			Assert.Equal(1, services.Length);
		}

		[Fact]
		public void ResolveAll_resolves_when_dependency_provideded_inline()
		{
			Kernel.Register(Component.For<ICommon>().ImplementedBy(typeof(CommonImplWithDependency)).Named("test"));
			var services = Kernel.ResolveAll<ICommon>(new Arguments().Insert("customer", new CustomerImpl()));
			Assert.Equal(1, services.Length);
		}

		[Fact]
		public void ResolveUsingAddionalParametersForConfigurationInsteadOfServices()
		{
			Kernel.Register(
				Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("cust").LifeStyle.Is(
					LifestyleType.Transient));

			var customer = Kernel.Resolve<ICustomer>("cust");
			Assert.Null(customer.Address);
			Assert.Null(customer.Name);
			Assert.Equal(0, customer.Age);

			var dictionary = new Dictionary<string, object> {{"Name", "name"}, {"Address", "address"}, {"Age", "18"}};
			customer = Kernel.Resolve<ICustomer>("cust", dictionary);

			Assert.Equal("name", customer.Name);
			Assert.Equal("address", customer.Address);
			Assert.Equal(18, customer.Age);
		}

		[Fact]
		public void ResolveViaGenerics()
		{
			Kernel.Register(Component.For<ICustomer>().ImplementedBy<CustomerImpl>().Named("cust"));
			Kernel.Register(Component.For<ICustomer>().ImplementedBy<CustomerImpl2>().Named("cust2"));
			var customer = Kernel.Resolve<ICustomer>("cust");

			var dictionary = new Dictionary<string, object>
			{
				{"name", "customer2Name"},
				{"address", "customer2Address"},
				{"age", 18}
			};
			var customer2 = Kernel.Resolve<ICustomer>("cust2", dictionary);

			Assert.Equal(customer.GetType(), typeof(CustomerImpl));
			Assert.Equal(customer2.GetType(), typeof(CustomerImpl2));
		}


		[Fact]
		public void Subsystems_are_case_insensitive()
		{
			Assert.NotNull(Kernel.GetSubSystem(SubSystemConstants.ConfigurationStoreKey));
			Assert.NotNull(Kernel.GetSubSystem(SubSystemConstants.ConfigurationStoreKey.ToLower()));
			Assert.NotNull(Kernel.GetSubSystem(SubSystemConstants.ConfigurationStoreKey.ToUpper()));
		}


		[Fact]
		public void UnregisteredComponentByService()
		{
			Assert.Throws<ComponentNotFoundException>(() =>
			{
				Kernel.Register(Component.For<CustomerImpl>().Named("key1"));
				Kernel.Resolve<IDisposable>();
			});
		}
	}
}