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

using System.Collections.Generic;
using System.Linq;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Registration
{
	public class DependsOnTestCase : AbstractContainerTestCase
	{
		[Test]
		public void Can_register_configuration_parameters_from_dynamic_parameters_inline()
		{
			Container.Register(Component.For<ClassWithArguments>()
				.DependsOn((k, d) => d.InsertAnonymous(new {arg1 = "a string", arg2 = 42})));

			var obj = Container.Resolve<ClassWithArguments>();

			Assert.AreEqual("a string", obj.Arg1);
			Assert.AreEqual(42, obj.Arg2);
		}

		[Test]
		public void Can_register_configuration_parameters_inline()
		{
			Container.Register(Component.For<ClassWithArguments>()
				.DependsOn(
					Dependency.OnConfigValue("arg1", "a string"),
					Dependency.OnConfigValue("arg2", "42")));

			var obj = Container.Resolve<ClassWithArguments>();

			Assert.AreEqual("a string", obj.Arg1);
			Assert.AreEqual(42, obj.Arg2);
		}

		[Test]
		public void Can_register_named_inline_dependency()
		{
			Container.Register(Component.For<ClassWithArguments>()
				.DependsOn(
					Dependency.OnValue("arg1", "a string"),
					Dependency.OnValue("arg2", 42)));

			var obj = Container.Resolve<ClassWithArguments>();

			Assert.AreEqual("a string", obj.Arg1);
			Assert.AreEqual(42, obj.Arg2);
		}

		[Test]
		public void Can_register_service_override_collection_named_via_names()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>().Named("a"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>().Named("c"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>().Named("b"),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection("services", "b", "a")));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_collection_named_via_types()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection("services", typeof(EmptyServiceB), typeof(EmptyServiceA))));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_collection_typed_via_names()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>().Named("a"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>().Named("c"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>().Named("b"),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection(typeof(ICollection<IEmptyService>), "b", "a")));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_collection_typed_via_names_generic()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>().Named("a"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>().Named("c"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>().Named("b"),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection<ICollection<IEmptyService>>("b", "a")));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_collection_typed_via_types()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection(typeof(ICollection<IEmptyService>), typeof(EmptyServiceB), typeof(EmptyServiceA))));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_collection_typed_via_types_generic()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceDecoratorViaProperty>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<CollectionDepAsConstructor>()
					.DependsOn(Dependency.OnComponentCollection<ICollection<IEmptyService>>(typeof(EmptyServiceB), typeof(EmptyServiceA))));

			var obj = Container.Resolve<CollectionDepAsConstructor>();
			Assert.AreEqual(2, obj.Services.Count);
			Assert.IsInstanceOf<EmptyServiceB>(obj.Services.First());
			Assert.IsInstanceOf<EmptyServiceA>(obj.Services.Last());
		}

		[Test]
		public void Can_register_service_override_named_via_name()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>().Named("a"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>().Named("b"),
				Component.For<UsesIEmptyService>()
					.DependsOn(Dependency.OnComponent("emptyService", "b")));

			var obj = Container.Resolve<UsesIEmptyService>();

			Assert.IsInstanceOf<EmptyServiceB>(obj.EmptyService);
		}

		[Test]
		public void Can_register_service_override_named_via_type()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<UsesIEmptyService>()
					.DependsOn(Dependency.OnComponent("emptyService", typeof(EmptyServiceB))));

			var obj = Container.Resolve<UsesIEmptyService>();

			Assert.IsInstanceOf<EmptyServiceB>(obj.EmptyService);
		}

		[Test]
		public void Can_register_service_override_typed_via_name()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>().Named("a"),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>().Named("b"),
				Component.For<UsesIEmptyService>()
					.DependsOn(Dependency.OnComponent(typeof(IEmptyService), "b")));

			var obj = Container.Resolve<UsesIEmptyService>();

			Assert.IsInstanceOf<EmptyServiceB>(obj.EmptyService);
		}

		[Test]
		public void Can_register_service_override_typed_via_type()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<UsesIEmptyService>()
					.DependsOn(Dependency.OnComponent(typeof(IEmptyService), typeof(EmptyServiceB))));

			var obj = Container.Resolve<UsesIEmptyService>();

			Assert.IsInstanceOf<EmptyServiceB>(obj.EmptyService);
		}

		[Test]
		public void Can_register_service_override_typed_via_type_generic()
		{
			Container.Register(
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceA>(),
				Component.For<IEmptyService>().ImplementedBy<EmptyServiceB>(),
				Component.For<UsesIEmptyService>()
					.DependsOn(Dependency.OnComponent<IEmptyService, EmptyServiceB>()));

			var obj = Container.Resolve<UsesIEmptyService>();

			Assert.IsInstanceOf<EmptyServiceB>(obj.EmptyService);
		}

		[Test]
		public void Can_register_typed_inline_dependency()
		{
			Container.Register(Component.For<ClassWithArguments>()
				.DependsOn(
					Dependency.OnValue(typeof(string), "a string"),
					Dependency.OnValue(typeof(int), 42)));

			var obj = Container.Resolve<ClassWithArguments>();

			Assert.AreEqual("a string", obj.Arg1);
			Assert.AreEqual(42, obj.Arg2);
		}

		[Test]
		public void Can_register_typed_inline_dependency_generic()
		{
			Container.Register(Component.For<ClassWithArguments>()
				.DependsOn(
					Dependency.OnValue<string>("a string"),
					Dependency.OnValue<int>(42)));

			var obj = Container.Resolve<ClassWithArguments>();

			Assert.AreEqual("a string", obj.Arg1);
			Assert.AreEqual(42, obj.Arg2);
		}
	}
}