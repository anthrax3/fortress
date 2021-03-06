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
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Tests.ClassComponents;
using Castle.Windsor.Tests.Facilities.Startable.Components;
using Xunit;

namespace Castle.Windsor.Tests.Facilities.Startable
{
	public class StartableFacilityTestCase
	{
	    private Assembly thisAssembly = typeof(StartableFacilityTestCase).GetTypeInfo().Assembly;

		public StartableFacilityTestCase()
		{
			kernel = new DefaultKernel();

			startableCreatedBeforeResolved = false;
		}

		private IKernel kernel;

		private bool startableCreatedBeforeResolved;

		private void OnNoInterfaceStartableComponentStarted(ComponentModel mode, object instance)
		{
			var startable = instance as NoInterfaceStartableComponent;

			Assert.NotNull(startable);
			Assert.True(startable.Started);
			Assert.False(startable.Stopped);

			startableCreatedBeforeResolved = true;
		}

		private void OnStartableComponentStarted(ComponentModel mode, object instance)
		{
			var startable = instance as StartableComponent;

			Assert.NotNull(startable);
			Assert.True(startable.Started);
			Assert.False(startable.Stopped);

			startableCreatedBeforeResolved = true;
		}

		[Fact]
		public void Startable_with_throwing_property_dependency()
		{
			HasThrowingPropertyDependency.InstancesStarted = 0;
			HasThrowingPropertyDependency.InstancesCreated = 0;
			kernel.AddFacility<StartableFacility>();
			kernel.Register(
				Component.For<ThrowsInCtor>(),
				Component.For<HasThrowingPropertyDependency>()
					.StartUsingMethod(x => x.Start)
			);

			Assert.Equal(1, HasThrowingPropertyDependency.InstancesCreated);
			Assert.Equal(1, HasThrowingPropertyDependency.InstancesStarted);
		}

		[Fact]
		public void Starts_component_without_start_method()
		{
			ClassWithInstanceCount.InstancesCount = 0;
			kernel.AddFacility<StartableFacility>(f => f.DeferredTryStart());
			kernel.Register(Component.For<ClassWithInstanceCount>().Start());
			Assert.Equal(1, ClassWithInstanceCount.InstancesCount);
		}

		[Fact]
		public void Starts_component_without_start_method_AllTypes()
		{
			ClassWithInstanceCount.InstancesCount = 0;
			kernel.AddFacility<StartableFacility>(f => f.DeferredTryStart());
			kernel.Register(Classes.FromAssembly(thisAssembly)
				.Where(t => t == typeof(ClassWithInstanceCount))
				.Configure(c => c.Start()));
			Assert.Equal(1, ClassWithInstanceCount.InstancesCount);
		}

		[Fact]
		public void TestComponentWithNoInterface()
		{
			kernel.ComponentCreated += OnNoInterfaceStartableComponentStarted;

			var compNode = new MutableConfiguration("component");
			compNode.Attributes["id"] = "b";
			compNode.Attributes["startable"] = "true";
			compNode.Attributes["startMethod"] = "Start";
			compNode.Attributes["stopMethod"] = "Stop";

			kernel.ConfigurationStore.AddComponentConfiguration("b", compNode);

			kernel.AddFacility<StartableFacility>();
			kernel.Register(Component.For<NoInterfaceStartableComponent>().Named("b"));

			Assert.True(startableCreatedBeforeResolved, "Component was not properly started");

			var component = kernel.Resolve<NoInterfaceStartableComponent>("b");

			Assert.NotNull(component);
			Assert.True(component.Started);
			Assert.False(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.True(component.Stopped);
		}

		[Fact]
		public void TestInterfaceBasedStartable()
		{
			kernel.ComponentCreated += OnStartableComponentStarted;

			kernel.AddFacility<StartableFacility>();

			kernel.Register(Component.For(typeof(StartableComponent)).Named("a"));

			Assert.True(startableCreatedBeforeResolved, "Component was not properly started");

			var component = kernel.Resolve<StartableComponent>("a");

			Assert.NotNull(component);
			Assert.True(component.Started);
			Assert.False(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.True(component.Stopped);
		}

		[Fact]
		public void TestStartableCallsStartOnlyOnceOnError()
		{
			StartableWithError.StartedCount = 0;
			kernel.AddFacility<StartableFacility>();

			var ex =
				Assert.Throws<Exception>(() =>
					kernel.Register(Component.For<StartableWithError>(),
						Component.For<ICommon>().ImplementedBy<CommonImpl1>()));

			// Every additional registration causes Start to be called again and again...
			Assert.Equal("This should go bonk", ex.Message);
			Assert.Equal(1, StartableWithError.StartedCount);
		}

		[Fact]
		public void TestStartableChainWithGenerics()
		{
			kernel.AddFacility<StartableFacility>();

			// Add parent. This has a dependency so won't be started yet.
			kernel.Register(Component.For(typeof(StartableChainParent)).Named("chainparent"));

			Assert.Equal(0, StartableChainDependency.startcount);
			Assert.Equal(0, StartableChainDependency.createcount);

			// Add generic dependency. This is not startable so won't get created. 
			kernel.Register(Component.For(typeof(StartableChainGeneric<>)).Named("chaingeneric"));

			Assert.Equal(0, StartableChainDependency.startcount);
			Assert.Equal(0, StartableChainDependency.createcount);

			// Add dependency. This will satisfy the dependency so everything will start.
			kernel.Register(Component.For(typeof(StartableChainDependency)).Named("chaindependency"));

			Assert.Equal(1, StartableChainParent.startcount);
			Assert.Equal(1, StartableChainParent.createcount);
			Assert.Equal(1, StartableChainDependency.startcount);
			Assert.Equal(1, StartableChainDependency.createcount);
			Assert.Equal(1, StartableChainGeneric<string>.createcount);
		}

		[Fact]
		public void TestStartableCustomDependencies()
		{
			kernel.ComponentCreated += OnStartableComponentStarted;

			kernel.AddFacility<StartableFacility>();

			kernel.Register(
				Component.For<StartableComponentCustomDependencies>()
					.Named("a")
					.DependsOn(Property.ForKey("config").Eq(1))
			);

			Assert.True(startableCreatedBeforeResolved, "Component was not properly started");

			var component = kernel.Resolve<StartableComponentCustomDependencies>("a");

			Assert.NotNull(component);
			Assert.True(component.Started);
			Assert.False(component.Stopped);

			kernel.ReleaseComponent(component);

			//Assert.True(component.Stopped);
		}

		[Fact]
		public void TestStartableExplicitFakeDependencies()
		{
			kernel.ComponentCreated += OnStartableComponentStarted;

			var dependsOnSomething = new DependencyModel(null, typeof(ICommon), false);

			kernel.AddFacility<StartableFacility>();
			kernel.Register(
				Component.For<StartableComponent>()
					.AddDescriptor(new AddDependency(dependsOnSomething))
			);

			Assert.False(startableCreatedBeforeResolved, "Component should not have started");

			kernel.Register(Component.For<ICommon>().ImplementedBy<CommonImpl1>());

			Assert.True(startableCreatedBeforeResolved, "Component was not properly started");
		}

		[Fact]
		public void TestStartableWithRegisteredCustomDependencies()
		{
			kernel.ComponentCreated += OnStartableComponentStarted;

			kernel.AddFacility<StartableFacility>();

			var dependencies = new Dictionary<string, object> {{"config", 1}};
			kernel.Register(Component.For<StartableComponentCustomDependencies>().DependsOn(dependencies));
			;

			Assert.True(startableCreatedBeforeResolved, "Component was not properly started");

			var component = kernel.Resolve<StartableComponentCustomDependencies>();

			Assert.NotNull(component);
			Assert.True(component.Started);
			Assert.False(component.Stopped);

			kernel.ReleaseComponent(component);
			//Assert.True(component.Stopped);
		}

		[Fact]
		public void Works_when_Start_and_Stop_methods_have_overloads()
		{
			kernel.AddFacility<StartableFacility>();
			kernel.Register(Component.For<WithOverloads>()
				.StartUsingMethod("Start")
				.StopUsingMethod("Stop"));
			var c = kernel.Resolve<WithOverloads>();
			Assert.True(c.StartCalled);
			kernel.ReleaseComponent(c);
			Assert.True(c.StopCalled);
		}
	}
}