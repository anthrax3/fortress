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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.DynamicProxy;
using Castle.Core.DynamicProxy.Generators;
using Castle.Core.DynamicProxy.Serialization;
using Castle.Core.Tests.InterClasses;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class ModuleScopeTestCase
	{
		private static void CheckSignedSavedAssembly(string path)
		{
			Assert.IsTrue(File.Exists(path));

			var assemblyName = AssemblyName.GetAssemblyName(path);

			var keyPairBytes = ModuleScope.GetKeyPair();
			var keyPair = new StrongNameKeyPair(keyPairBytes);
			var loadedPublicKey = assemblyName.GetPublicKey();

			Assert.AreEqual(keyPair.PublicKey.Length, loadedPublicKey.Length);
			for (var i = 0; i < keyPair.PublicKey.Length; ++i)
				Assert.AreEqual(keyPair.PublicKey[i], loadedPublicKey[i]);
		}

		private static void CheckUnsignedSavedAssembly(string path)
		{
			Assert.IsTrue(File.Exists(path));
		}

		private delegate Type ProxyCreator(IProxyBuilder proxyBuilder);

		private void CheckLoadAssemblyIntoCache(ProxyCreator creator)
		{
			var savedScope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(savedScope);

			var cp = creator(builder);
			Assert.AreSame(cp, creator(builder));

			var path = savedScope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var newScope = new ModuleScope(false);
				var newBuilder = new DefaultProxyBuilder(newScope);

				var assembly = Assembly.LoadFrom((string) args[0]);
				newScope.LoadAssemblyIntoCache(assembly);

				var loadedCP = assembly.GetType((string) args[1]);
				Assert.AreSame(loadedCP, ((ProxyCreator) args[2])(newBuilder));
				Assert.AreEqual(assembly, ((ProxyCreator) args[2])(newBuilder).Assembly);
			}, path, cp.FullName, creator);

			File.Delete(path);
		}

		[Test]
		public void CacheMappingsHoldTypes()
		{
			var scope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(scope);
			var classProxy = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, ProxyGenerationOptions.Default);

			var savedPath = scope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
				{
					var assembly = Assembly.LoadFrom((string) args[0]);
					var attribute = (CacheMappingsAttribute) assembly.GetCustomAttributes(typeof(CacheMappingsAttribute), false)[0];
					var entries = attribute.GetDeserializedMappings();
					Assert.AreEqual(1, entries.Count);
					var key = new CacheKey(typeof(object), new Type[0], ProxyGenerationOptions.Default);
					Assert.IsTrue(entries.ContainsKey(key));
					Assert.AreEqual(args[1], entries[key]);
				}, savedPath, classProxy.FullName);
		}

		[Test]
		public void CacheMappingsHoldTypesShouldSurviveAThreadBashing()
		{
			var tasks = new List<Task>();
			for (int i = 0; i < 10; i++)
			{
				var task = Task.Factory.StartNew(CacheMappingsHoldTypes);
				tasks.Add(task);
			}
			Task.WaitAll(tasks.ToArray());
		}

		[Test]
		public void DefaultProxyBuilderWithSpecificScope()
		{
			var scope = new ModuleScope(false);
			var builder = new DefaultProxyBuilder(scope);
			Assert.AreSame(scope, builder.ModuleScope);
		}

		[Test]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedStrongName()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithWeakName();

			Assert.Throws<InvalidOperationException>(() => scope.SaveAssembly(true));
		}

		[Test]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedWeakName()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithStrongName();

			Assert.Throws<InvalidOperationException>(() => scope.SaveAssembly(false));
		}

		[Test]
		public void ExplicitSaveWorksEvenWhenMultipleAssembliesGenerated()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			scope.SaveAssembly(true);

            CheckSignedSavedAssembly(scope.StrongNamedModuleName);

			scope.SaveAssembly(false);

            CheckUnsignedSavedAssembly(scope.StrongNamedModuleName);
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
		public void LoadAssemblyIntoCache_CreateClassProxy()
		{
			CheckLoadAssemblyIntoCache(builder => builder.CreateClassProxyType(typeof(object), null, ProxyGenerationOptions.Default));
		}

		[Test]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithoutTarget()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithoutTarget(typeof(IServiceProvider), new Type[0],
						ProxyGenerationOptions.Default);
				});
		}

		[Test]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithTarget()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithTarget(typeof(IMyInterface2), new Type[0], typeof(MyInterfaceImpl),
						ProxyGenerationOptions.Default);
				});
		}

		[Test]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithTargetInterface()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithTargetInterface(typeof(IMyInterface2), null,
						ProxyGenerationOptions.Default);
				});
		}

		[Test]
		public void LoadAssemblyIntoCache_DifferentGenerationOptions()
		{
			var savedScope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(savedScope);

			var options1 = new ProxyGenerationOptions();
			options1.AddMixinInstance(new DateTime());
			var options2 = ProxyGenerationOptions.Default;

			var cp1 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options1);
			var cp2 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options2);
			Assert.AreNotSame(cp1, cp2);
			Assert.AreSame(cp1, builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options1));
			Assert.AreSame(cp2, builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options2));

			var path = savedScope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var newScope = new ModuleScope(false);
				var newBuilder = new DefaultProxyBuilder(newScope);

				var assembly = Assembly.LoadFrom((string) args[0]);
				newScope.LoadAssemblyIntoCache(assembly);

				var newOptions1 = new ProxyGenerationOptions();
				newOptions1.AddMixinInstance(new DateTime());
				var newOptions2 = ProxyGenerationOptions.Default;

				var loadedCP1 = newBuilder.CreateClassProxyType(typeof(object),
					Type.EmptyTypes,
					newOptions1);
				var loadedCP2 = newBuilder.CreateClassProxyType(typeof(object),
					Type.EmptyTypes,
					newOptions2);
				Assert.AreNotSame(loadedCP1, loadedCP2);
				Assert.AreEqual(assembly, loadedCP1.Assembly);
				Assert.AreEqual(assembly, loadedCP2.Assembly);
			}, path);

			File.Delete(path);
		}

		[Test]
		public void LoadAssemblyIntoCache_InvalidAssembly()
		{
			var newScope = new ModuleScope(false);

			Assert.Throws<ArgumentException>(() =>
				newScope.LoadAssemblyIntoCache(Assembly.GetExecutingAssembly())
			);
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

		[Test]
		public void SavedAssemblyHasCacheMappings()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithWeakName();

			var savedPath = scope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
				{
					var assembly = Assembly.LoadFrom((string) args[0]);
					Assert.IsTrue(assembly.IsDefined(typeof(CacheMappingsAttribute), false));
				},
				savedPath);

			File.Delete(savedPath);
		}

		[Test]
		public void SaveReturnsNullWhenNoModuleObtained()
		{
			var scope = new ModuleScope(true);
			Assert.IsNull(scope.SaveAssembly());
		}

		[Test]
		public void SaveThrowsWhenMultipleAssembliesGenerated()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			Assert.Throws<InvalidOperationException>(() => scope.SaveAssembly());
		}

		[Test]
		public void SaveWithFlagFalseDoesntThrowsWhenMultipleAssembliesGenerated()
		{
			var scope = new ModuleScope(false);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			scope.SaveAssembly();
		}
	}
}