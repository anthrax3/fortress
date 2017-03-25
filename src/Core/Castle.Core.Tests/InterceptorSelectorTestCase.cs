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
using Castle.Core.DynamicProxy;
using Castle.Core.DynamicProxy.Generators;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.InterClasses;
using Castle.Core.Tests.Interfaces;
using Xunit;


namespace Castle.Core.Tests
{
	public class InterceptorSelectorTestCase : CoreBaseTestCase
	{
		private interface PrivateInterface
		{
		}

		private class PrivateClass : PrivateInterface
		{
		}

		[Fact]
		public void BasicCase()
		{
			var options = new ProxyGenerationOptions();
			options.Selector = new AllInterceptorSelector();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options, new StandardInterceptor()) as ISimpleInterface;
			Assert.NotNull(target);
			target.Do();
		}

		[Fact]
		public void Can_proxy_generic_interface()
		{
			generator.CreateInterfaceProxyWithTarget<IList<object>>(new List<object>());
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_ClassProxy()
		{
			var someInstanceOfProxyWithoutSelector = (Component2) generator.CreateClassProxy(typeof(Component2), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (Component2) generator.CreateClassProxy(typeof(Component2),
				new ProxyGenerationOptions {Selector = new AllInterceptorSelector()},
				new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_ClassProxyWithTarget()
		{
			var someInstanceOfProxyWithoutSelector = (Component2) generator.CreateClassProxyWithTarget(typeof(Component2), new Component2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (Component2) generator.CreateClassProxyWithTarget(typeof(Component2), new Component2(),
				new ProxyGenerationOptions {Selector = new AllInterceptorSelector()},
				new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithoutTarget()
		{
			var someInstanceOfProxyWithoutSelector = (IService2) generator.CreateInterfaceProxyWithoutTarget(typeof(IService2), new DoNothingInterceptor());
			var someInstanceOfProxyWithSelector = (IService2) generator.CreateInterfaceProxyWithoutTarget(typeof(IService2), new ProxyGenerationOptions
			{
				Selector = new AllInterceptorSelector()
			}, new DoNothingInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTarget()
		{
			var someInstanceOfProxyWithoutSelector = (IService2) generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (IService2) generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
				new ProxyGenerationOptions {Selector = new AllInterceptorSelector()},
				new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTarget2()
		{
			var someInstanceOfProxyWithSelector1 = (IService2) generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
				new ProxyGenerationOptions {Selector = new SelectorWithState(1)},
				new StandardInterceptor());
			var someInstanceOfProxyWithSelector2 = (IService2) generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
				new ProxyGenerationOptions {Selector = new SelectorWithState(2)},
				new StandardInterceptor());

			Assert.Same(someInstanceOfProxyWithSelector1.GetType(), someInstanceOfProxyWithSelector2.GetType());
		}

		[Fact]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTargetInterface()
		{
			var someInstanceOfProxyWithoutSelector = (IService2) generator.CreateInterfaceProxyWithTargetInterface(typeof(IService2), new Service2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (IService2) generator.CreateInterfaceProxyWithTargetInterface(typeof(IService2), new Service2(),
				new ProxyGenerationOptions {Selector = new AllInterceptorSelector()},
				new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_inaccessible_type_argument()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithTarget<IList<PrivateInterface>>(new List<PrivateInterface>()));
		}

		[Fact]
		public void Cannot_proxy_generic_interface_with_type_argument_that_has_inaccessible_type_argument()
		{
			Assert.Throws<GeneratorException>(() => generator.CreateInterfaceProxyWithTarget<IList<IList<PrivateInterface>>>(new List<IList<PrivateInterface>>()));
		}

		[Fact]
		public void Cannot_proxy_inaccessible_interface()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithTarget<PrivateInterface>(new PrivateClass()));
		}

		[Fact]
		public void SelectorWorksForGenericMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(IGenericInterface), new GenericClass(), options,
				new AddTwoInterceptor(),
				countingInterceptor) as IGenericInterface;
			Assert.NotNull(target);
			var result = target.GenericMethod<int>();
			Assert.Equal(1, countingInterceptor.Count);
			Assert.Equal(0, result);
			var result2 = target.GenericMethod<string>();
			Assert.Equal(2, countingInterceptor.Count);
			Assert.Equal(default(string), result2);
		}

		[Fact]
		public void SelectorWorksForMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options,
				new AddTwoInterceptor(), countingInterceptor) as ISimpleInterface;
			Assert.NotNull(target);
			var result = target.Do();
			Assert.Equal(3, result);
			Assert.Equal(1, countingInterceptor.Count);
		}

		[Fact]
		public void SelectorWorksForMixins()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SimpleClass());
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
				new SimpleClassWithProperty(), options,
				new AddTwoInterceptor(),
				countingInterceptor) as ISimpleInterface;
			Assert.NotNull(target);
			var result = target.Do();
			Assert.Equal(3, result);
			Assert.Equal(1, countingInterceptor.Count);
		}

		[Fact]
		public void SelectorWorksForMultipleGenericMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(IMultiGenericInterface), new MultiGenericClass(),
				options,
				new AddTwoInterceptor(),
				countingInterceptor) as IMultiGenericInterface;
			Assert.NotNull(target);
			var result = target.Method<int, string>("ignored");
			Assert.Equal(1, countingInterceptor.Count);
			Assert.Equal(0, result);
			var result2 = target.Method<string, int>(0);
			Assert.Equal(2, countingInterceptor.Count);
			Assert.Equal(default(string), result2);
		}

		[Fact]
		public void SelectorWorksForProperties()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
				new SimpleClassWithProperty(), options,
				new AddTwoInterceptor(),
				countingInterceptor) as ISimpleInterfaceWithProperty;
			Assert.NotNull(target);
			var result = target.Age;
			Assert.Equal(5, result);
			Assert.Equal(1, countingInterceptor.Count);
		}

		[Fact]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_ClassProxy()
		{
			var options1 = new ProxyGenerationOptions {Selector = new AllInterceptorSelector()};
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateClassProxy(typeof(ServiceClass), Type.EmptyTypes, options1);
			var proxy2 = generator.CreateClassProxy(typeof(ServiceClass), Type.EmptyTypes, options2);
			(proxy1 as ServiceClass).Sum(2, 2);
			(proxy2 as ServiceClass).Sum(2, 2);

			Assert.Same(proxy1.GetType(), proxy2.GetType());
		}

		[Fact]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithoutTarget()
		{
			var options1 = new ProxyGenerationOptions {Selector = new AllInterceptorSelector()};
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<SetReturnValueInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), Type.EmptyTypes, options1, new SetReturnValueInterceptor(2));
			var proxy2 = generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), Type.EmptyTypes, options2, new SetReturnValueInterceptor(2));
			(proxy1 as IOne).OneMethod();
			(proxy2 as IOne).OneMethod();

			Assert.Same(proxy1.GetType(), proxy2.GetType());
		}

		[Fact]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithTarget()
		{
			var options1 = new ProxyGenerationOptions {Selector = new AllInterceptorSelector()};
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), options1);
			var proxy2 = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), options2);
			proxy1.OneMethod();
			proxy2.OneMethod();

			Assert.Same(proxy1.GetType(), proxy2.GetType());
		}

		[Fact]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithTargetInterface()
		{
			var options1 = new ProxyGenerationOptions {Selector = new AllInterceptorSelector()};
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), options1);
			var proxy2 = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), options2);
			proxy1.OneMethod();
			proxy2.OneMethod();

			Assert.Same(proxy1.GetType(), proxy2.GetType());
		}
	}
}