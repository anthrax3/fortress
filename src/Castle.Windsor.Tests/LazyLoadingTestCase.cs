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
using System.Threading;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.MicroKernel.Resolvers;
using Castle.Windsor.Tests.Components;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
	[TestFixture]
	public class LazyLoadingTestCase : AbstractContainerTestCase
	{
		[Test]
		public void Can_Lazily_resolve_component()
		{
			Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LoaderForDefaultImplementations>());
			var service = Container.Resolve("foo", typeof(IHasDefaultImplementation));
			Assert.IsNotNull(service);
			Assert.IsInstanceOf<Implementation>(service);
		}

		[Test]
		public void Can_lazily_resolve_dependency()
		{
			Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LoaderForDefaultImplementations>(),
				Component.For<UsingLazyComponent>());
			var component = Container.Resolve<UsingLazyComponent>();
			Assert.IsNotNull(component.Dependency);
		}

		[Test]
		public void Can_lazily_resolve_explicit_dependency()
		{
			Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LoaderUsingDependency>());
			var component = Container.Resolve<UsingString>(new Arguments().Insert("parameter", "Hello"));
			Assert.AreEqual("Hello", component.Parameter);
		}

		[Test]
		public void Component_loaded_lazily_can_have_lazy_dependencies()
		{
			Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<ABLoader>());
			Container.Resolve<B>();
		}

		[Test]
		[Timeout(2000)]
		public void Loaders_are_thread_safe()
		{
			Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<SlowLoader>());
			var @event = new ManualResetEvent(false);
			int[] count = {10};
			Exception exception = null;
			for (var i = 0; i < count[0]; i++)
				ThreadPool.QueueUserWorkItem(o =>
					{
						try
						{
							Container.Resolve<Implementation>("not registered");
							if (Interlocked.Decrement(ref count[0]) == 0)
								@event.Set();
						}
						catch (Exception e)
						{
							exception = e;
							// this is required because NUnit does not consider it a failure when
							// an exception is thrown on a non-main thread and therfore it waits.
							@event.Set();
						}
					}
				);
			@event.WaitOne();
			Assert.IsNull(exception);
			Assert.AreEqual(0, count[0]);
		}

		[Test]
		public void Loaders_only_triggered_when_resolving()
		{
			var loader = new ABLoaderWithGuardClause();
			Container.Register(Component.For<ILazyComponentLoader>().Instance(loader),
				Component.For<B>());

			loader.CanLoadNow = true;

			Container.Resolve<B>();
		}

		[Test]
		public void Loaders_with_dependencies_dont_overflow_the_stack()
		{
			Container.Register(Component.For<LoaderWithDependency>());

			Assert.Throws<ComponentNotFoundException>(() =>
				Container.Resolve<ISpecification>("some not registered service"));
		}
	}
}