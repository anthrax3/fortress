// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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

using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Tests.Generics;
using Castle.Windsor.Tests.TestImplementationsOfExtensionPoints;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
	[TestFixture]
	public class GenericImplementationWithGreaterArityThanServiceTestCase : AbstractContainerTestCase
	{
		[Test]
		public void Can_create_component_with_generic_impl_for_non_generic_services()
		{
			Container.Register(Component.For<IService>().ImplementedBy(typeof(ServiceImplGeneric<>), new UseStringGenericStrategy()));

			var item = Container.Resolve<IService>();

			Assert.IsInstanceOf<ServiceImplGeneric<string>>(item);
		}

		[Test]
		public void Can_create_component_with_simple_double_generic_impl_for_multi_class_registration()
		{
			Container.Register(
				Classes.FromThisAssembly().BasedOn(typeof(Generics.IRepository<>))
					.If(t => t == typeof(DoubleGenericRepository<,>))
					.WithServiceBase()
					.Configure(
						c => c.ExtendedProperties(
							Property.ForKey(Constants.GenericImplementationMatchingStrategy)
								.Eq(new DuplicateGenerics()))));

			var repository = Container.Resolve<Generics.IRepository<A>>();

			Assert.IsInstanceOf<DoubleGenericRepository<A, A>>(repository);
		}

		[Test]
		public void Can_create_component_with_simple_double_generic_impl_for_single_generic_service()
		{
			Container.Register(Component.For(typeof(Generics.IRepository<>)).ImplementedBy(typeof(DoubleGenericRepository<,>))
				.ExtendedProperties(Property.ForKey(Constants.GenericImplementationMatchingStrategy).Eq(new DuplicateGenerics())));

			var repository = Container.Resolve<Generics.IRepository<A>>();

			Assert.IsInstanceOf<DoubleGenericRepository<A, A>>(repository);
		}

		[Test]
		public void Can_create_component_with_simple_double_generic_impl_for_single_generic_service_via_ImplementedBy()
		{
			Container.Register(Component.For(typeof(Generics.IRepository<>)).ImplementedBy(typeof(DoubleGenericRepository<,>), new DuplicateGenerics()));

			var repository = Container.Resolve<Generics.IRepository<A>>();

			Assert.IsInstanceOf<DoubleGenericRepository<A, A>>(repository);
		}
	}
}