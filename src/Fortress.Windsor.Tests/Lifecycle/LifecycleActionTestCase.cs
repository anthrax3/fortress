// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

using Castle.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Tests.Generics;
using Xunit;

namespace Castle.Windsor.Tests.Lifecycle
{
	
	public class LifecycleActionTestCase : AbstractContainerTestCase
	{
		[Fact]
		public void Can_apply_OnCreate_to_open_generic_components()
		{
			var called = false;
			Container.Register(Component.For(typeof(GenericA<>))
				.OnCreate((kernel, instance) => { called = true; }));
			Container.Resolve<GenericA<int>>();
			Assert.True(called);
		}

		[Fact]
		public void Can_apply_OnDestroy_to_open_generic_components()
		{
			var called = false;
			Container.Register(Component.For(typeof(GenericA<>))
				.LifestyleTransient()
				.OnDestroy((kernel, instance) => { called = true; }));
			var item = Container.Resolve<GenericA<int>>();
			Container.Release(item);
			Assert.True(called);
		}

		[Fact]
		public void Can_mix_vaious_overloads_OnCreate()
		{
			Container.Register(Component.For<IService>().ImplementedBy<MyService>()
				.OnCreate((kernel, instance) => instance.Name += "a")
				.OnCreate(instance => instance.Name += "b"));
			var service = Container.Resolve<IService>();
			Assert.True(service.Name == "ba");
		}

		[Fact]
		public void Can_mix_vaious_overloads_OnDestroy()
		{
			Container.Register(Component.For<IService>().ImplementedBy<MyService>()
				.LifestyleTransient()
				.OnDestroy((kernel, instance) => instance.Name += "a")
				.OnDestroy(instance => instance.Name += "b"));
			var service = Container.Resolve<IService>();
			Assert.Equal(string.Empty, service.Name);

			Container.Release(service);
			Assert.Equal("ba", service.Name);
		}

		[Fact]
		public void CanModify_when_singleton()
		{
			Container.Register(Component.For<IService>().ImplementedBy<MyService>()
				.OnCreate((kernel, instance) => instance.Name += "a"));
			var service = Container.Resolve<IService>();
			Assert.True(service.Name == "a");
			service = Container.Resolve<IService>();
			Assert.True(service.Name == "a");
		}

		[Fact]
		public void CanModify_when_singleton_multiple_ordered()
		{
			Container.Register(Component.For<IService>().ImplementedBy<MyService>()
				.OnCreate((kernel, instance) => instance.Name += "a",
					(kernel, instance) => instance.Name += "b"));
			var service = Container.Resolve<IService>();
			Assert.True(service.Name == "ab");
			service = Container.Resolve<IService>();
			Assert.True(service.Name == "ab");
		}

		[Fact]
		public void CanModify_when_transient()
		{
			MyService2.staticname = "";
			Container.Register(Component.For<IService2>().ImplementedBy<MyService2>()
				.LifeStyle.Transient.OnCreate((kernel, instance) => instance.Name += "a"));
			var service = Container.Resolve<IService2>();
			Assert.True(service.Name == "a");
			service = Container.Resolve<IService2>();
			Assert.True(service.Name == "aa");
		}

		[Fact]
		public void CanModify_when_transient_multiple_ordered()
		{
			MyService2.staticname = "";
			Container.Register(Component.For<IService2>().ImplementedBy<MyService2>()
				.LifeStyle.Transient.OnCreate((kernel, instance) => instance.Name += "a",
					(kernel, instance) => instance.Name += "b"));
			var service = Container.Resolve<IService2>();
			Assert.True(service.Name == "ab");

			service = Container.Resolve<IService2>();
			Assert.True(service.Name == "abab");
		}

		[Fact]
		[Bug("IOC-326")]
		public void OnDestroy_called_before_disposal()
		{
			var wasDisposed = false;
			Container.Register(Component.For<ADisposable>()
				.LifeStyle.Transient
				.OnDestroy((k, i) => { wasDisposed = i.Disposed; }));

			var a = Container.Resolve<ADisposable>();
			Container.Release(a);

			Assert.False(wasDisposed);
			Assert.True(a.Disposed);
		}

		[Fact]
		public void OnDestroy_called_on_release()
		{
			var called = false;
			Container.Register(Component.For<A>()
				.LifeStyle.Transient
				.OnDestroy((k, i) => { called = true; }));

			Assert.False(called);
			var a = Container.Resolve<A>();
			Container.Release(a);

			Assert.True(called);
		}

		[Fact]
		public void OnDestroy_makes_transient_simple_component_tracked()
		{
			Container.Register(Component.For<A>()
				.LifeStyle.Transient
				.OnDestroy((k, i) => { }));

			var a = Container.Resolve<A>();
			Assert.True(Kernel.ReleasePolicy.HasTrack(a));
			Container.Release(a);
		}

		[Fact]
		public void Works_for_components_obtained_via_factory()
		{
			Container.Register(Component.For<IService>()
				.UsingFactoryMethod(() => new MyService())
				.OnCreate((kernel, instance) => instance.Name += "a"));

			var service = Container.Resolve<IService>();

			Assert.True(service.Name == "a");
		}
	}
}