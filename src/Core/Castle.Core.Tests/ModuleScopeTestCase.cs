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

using Castle.Core.DynamicProxy;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class ModuleScopeTestCase
	{
		[Test]
		public void DefaultProxyBuilderWithSpecificScope()
		{
			var scope = new ModuleScope(false);
			var builder = new DefaultProxyBuilder(scope);
			Assert.AreSame(scope, builder.ModuleScope);
		}

		[Test]
		public void GeneratedAssembliesDefaultName()
		{
			var scope = new ModuleScope();

            var strong = scope.ObtainDynamicModuleWithStrongName();

            var weak = scope.ObtainDynamicModuleWithWeakName();

			Assert.AreEqual(scope.StrongAssemblyName, strong.Assembly.GetName().Name);

            Assert.AreEqual(scope.WeakAssemblyName, weak.Assembly.GetName().Name);
		}


		[Test]
		public void ModuleScopeCanHandleSignedAndUnsignedInParallel()
		{
			var scope = new ModuleScope();
			Assert.IsNull(scope.StrongNamedModule);
			Assert.IsNull(scope.WeakNamedModule);

			var one = scope.ObtainDynamicModuleWithStrongName();
			Assert.IsNotNull(scope.StrongNamedModule);
			Assert.IsNull(scope.WeakNamedModule);
			Assert.AreSame(one, scope.StrongNamedModule);

			var two = scope.ObtainDynamicModuleWithWeakName();
			Assert.IsNotNull(scope.StrongNamedModule);
			Assert.IsNotNull(scope.WeakNamedModule);
			Assert.AreSame(two, scope.WeakNamedModule);

			Assert.AreNotSame(one, two);
			Assert.AreNotSame(one.Assembly, two.Assembly);

			var three = scope.ObtainDynamicModuleWithStrongName();
			var four = scope.ObtainDynamicModuleWithWeakName();

			Assert.AreSame(one, three);
			Assert.AreSame(two, four);
		}

		[Test]
		public void ModuleScopeStoresModuleBuilder()
		{
			var scope = new ModuleScope();
			var one = scope.ObtainDynamicModuleWithStrongName();
			var two = scope.ObtainDynamicModuleWithStrongName();

			Assert.AreSame(one, two);
			Assert.AreSame(one.Assembly, two.Assembly);
		}
	}
}