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

using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using Xunit;

namespace Castle.Windsor.Tests
{
	
	public class HandlerExtensionsTestCase : AbstractContainerTestCase
	{
		private ComponentRegistration<A> AddResolveExtensions(ComponentRegistration<A> componentRegistration,
			params IResolveExtension[] items)
		{
			var resolveExtensions = new List<IResolveExtension>();
			foreach (var item in items.Distinct())
				resolveExtensions.Add(item);
			return componentRegistration.ExtendedProperties(Property.ForKey("Castle.ResolveExtensions").Eq(resolveExtensions));
		}

		private ComponentRegistration<TComponent> WithReleaseExtensions<TComponent>(
			ComponentRegistration<TComponent> componentRegistration, params IReleaseExtension[] items)
			where TComponent : class
		{
			var releaseExtensions = new List<IReleaseExtension>();
			foreach (var item in items.Distinct())
				releaseExtensions.Add(item);
			return componentRegistration.ExtendedProperties(Property.ForKey("Castle.ReleaseExtensions").Eq(releaseExtensions));
		}

		[Fact]
		public void Can_chain_extensions()
		{
			var a = new A();
			var collector = new CollectItemsExtension();
			Kernel.Register(AddResolveExtensions(Component.For<A>(), collector, new ReturnAExtension(a)));
			Kernel.Resolve<A>();
			Assert.Same(a, collector.ResolvedItems.Single());
		}

		[Fact]
		public void Can_intercept_entire_resolution()
		{
			var a = new A();
			var componentRegistration = Component.For<A>();
			Kernel.Register(AddResolveExtensions(componentRegistration, new ReturnAExtension(a)));
			var resolvedA = Kernel.Resolve<A>();
			Assert.Same(a, resolvedA);
		}

		[Fact]
		public void Can_proceed_and_inspect_released_value_on_singleton()
		{
			var collector = new CollectItemsExtension();
			Kernel.Register(WithReleaseExtensions(Component.For<DisposableFoo>(), collector));
			var a = Kernel.Resolve<DisposableFoo>();
			Kernel.Dispose();
			Assert.Equal(1, collector.ReleasedItems.Count);
			Assert.Same(a, collector.ReleasedItems[0]);
		}

		[Fact]
		public void Can_proceed_and_inspect_released_value_on_transinet()
		{
			var collector = new CollectItemsExtension();
			Kernel.Register(WithReleaseExtensions(Component.For<DisposableFoo>().LifeStyle.Transient, collector));
			var a = Kernel.Resolve<DisposableFoo>();
			Kernel.ReleaseComponent(a);
			Assert.Equal(1, collector.ReleasedItems.Count);
			Assert.Same(a, collector.ReleasedItems[0]);
		}

		[Fact]
		public void Can_proceed_and_inspect_returned_value()
		{
			var collector = new CollectItemsExtension();
			Kernel.Register(AddResolveExtensions(Component.For<A>(), collector));
			Kernel.Resolve<A>();
			var resolved = Kernel.Resolve<A>();
			Assert.Equal(2, collector.ResolvedItems.Count);
			Assert.Same(resolved, collector.ResolvedItems[0]);
			Assert.Same(resolved, collector.ResolvedItems[1]);
		}

		[Fact]
		public void Can_replace_returned_value()
		{
			var a = new A();
			var componentRegistration = Component.For<A>();
			Kernel.Register(AddResolveExtensions(componentRegistration, new ReturnAExtension(a, true)));
			var resolvedA = Kernel.Resolve<A>();
			Assert.Same(a, resolvedA);
		}
	}
}