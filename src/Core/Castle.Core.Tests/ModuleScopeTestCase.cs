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

using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class ModuleScopeTestCase
	{
		[Fact]
		public void DefaultProxyBuilderWithSpecificScope()
		{
			var scope = new ModuleScope(false);
			var builder = new DefaultProxyBuilder(scope);
			Assert.Same(scope, builder.ModuleScope);
		}

		[Fact]
		public void GeneratedAssembliesDefaultName()
		{
			var scope = new ModuleScope();

            var strong = scope.ObtainDynamicModuleWithStrongName();

            var weak = scope.ObtainDynamicModuleWithWeakName();

			Assert.Equal(scope.StrongAssemblyName, strong.Assembly.GetName().Name);

            Assert.Equal(scope.WeakAssemblyName, weak.Assembly.GetName().Name);
		}


		[Fact]
		public void ModuleScopeCanHandleSignedAndUnsignedInParallel()
		{
			var scope = new ModuleScope();
			Assert.Null(scope.StrongNamedModule);
			Assert.Null(scope.WeakNamedModule);

			var one = scope.ObtainDynamicModuleWithStrongName();
			Assert.NotNull(scope.StrongNamedModule);
			Assert.Null(scope.WeakNamedModule);
			Assert.Same(one, scope.StrongNamedModule);

			var two = scope.ObtainDynamicModuleWithWeakName();
			Assert.NotNull(scope.StrongNamedModule);
			Assert.NotNull(scope.WeakNamedModule);
			Assert.Same(two, scope.WeakNamedModule);

			Assert.NotSame(one, two);
			Assert.NotSame(one.Assembly, two.Assembly);

			var three = scope.ObtainDynamicModuleWithStrongName();
			var four = scope.ObtainDynamicModuleWithWeakName();

			Assert.Same(one, three);
			Assert.Same(two, four);
		}

		[Fact]
		public void ModuleScopeStoresModuleBuilder()
		{
			var scope = new ModuleScope();
			var one = scope.ObtainDynamicModuleWithStrongName();
			var two = scope.ObtainDynamicModuleWithStrongName();

			Assert.Same(one, two);
			Assert.Same(one.Assembly, two.Assembly);
		}
	}
}