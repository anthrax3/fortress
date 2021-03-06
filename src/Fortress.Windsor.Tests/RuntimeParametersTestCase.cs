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

using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Tests.RuntimeParameters;
using Xunit;

namespace Castle.Windsor.Tests
{
	
	public class RuntimeParametersTestCase : AbstractContainerTestCase
	{
		private readonly Dictionary<string, object> dependencies = new Dictionary<string, object> {{"cc", new CompC(12)}, {"myArgument", "ernst"}};

		private void AssertDependencies(CompB compb)
		{
			Assert.NotNull(compb);

			Assert.NotNull(compb.Compc);
			Assert.True(compb.MyArgument != string.Empty, "MyArgument property should not be empty");

			Assert.Same(dependencies["cc"], compb.Compc);
			Assert.True("ernst".Equals(compb.MyArgument),
				string.Format("The MyArgument property of compb should be equal to ernst, found {0}", compb.MyArgument));
		}

		[Fact]
		public void AddingDependencyToServiceWithCustomDependency()
		{
			var kernel = new DefaultKernel();
			kernel.Register(Component.For<NeedClassWithCustomerDependency>(),
				Component.For<HasCustomDependency>().DependsOn(new Dictionary<object, object> {{"name", new CompA()}}));

			Assert.Equal(HandlerState.Valid, kernel.GetHandler(typeof(HasCustomDependency)).CurrentState);
			Assert.NotNull(kernel.Resolve(typeof(NeedClassWithCustomerDependency)));
		}


		[Fact]
		public void Parameter_takes_precedence_over_registered_service()
		{
			Container.Register(Component.For<CompA>(),
				Component.For<CompB>().DependsOn(Dependency.OnValue<string>("some string")),
				Component.For<CompC>().Instance(new CompC(0)));

			var c2 = new CompC(42);
			var args = new Arguments(new object[] {c2});
			var b = Container.Resolve<CompB>(args);

			Assert.Same(c2, b.Compc);
		}

		[Fact]
		public void ParametersPrecedence()
		{
			Container.Register(Component.For<CompA>().Named("compa"),
				Component.For<CompB>().Named("compb").DependsOn(dependencies));

			var instance_with_model = Container.Resolve<CompB>();
			Assert.Same(dependencies["cc"], instance_with_model.Compc);

			var deps2 = new Dictionary<string, object> {{"cc", new CompC(12)}, {"myArgument", "ayende"}};

			var instance_with_args = Container.Resolve<CompB>(deps2);

			Assert.Same(deps2["cc"], instance_with_args.Compc);
			Assert.Equal("ayende", instance_with_args.MyArgument);
		}

		[Fact]
		public void ResolveUsingParameters()
		{
			Container.Register(Component.For<CompA>().Named("compa"),
				Component.For<CompB>().Named("compb"));
			var compb = Container.Resolve<CompB>(dependencies);

			AssertDependencies(compb);
		}

		[Fact]
		public void ResolveUsingParametersWithinTheHandler()
		{
			Container.Register(Component.For<CompA>().Named("compa"),
				Component.For<CompB>().Named("compb").DependsOn(dependencies));

			var compb = Container.Resolve<CompB>();

			AssertDependencies(compb);
		}

		[Fact]
		public void WillAlwaysResolveCustomParameterFromServiceComponent()
		{
			Container.Register(Component.For<CompA>(),
				Component.For<CompB>().DependsOn(new {myArgument = "foo"}),
				Component.For<CompC>().DependsOn(new {test = 15}));
			var b = Kernel.Resolve<CompB>();
			Assert.NotNull(b);
			Assert.Equal(15, b.Compc.test);
		}
	}
}