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

using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.Interceptors;
using Castle.DynamicProxy;
using Xunit;

namespace Castle.Core.Tests
{
	public class AccessLevelTestCase : CoreBaseTestCase
	{
		internal class InternalClass
		{
		}

		[Fact]
		public void InternalConstructorIsNotReplicated()
		{
			var proxy = generator.CreateClassProxy(typeof(Dictionary<int, object>), new StandardInterceptor());
			Assert.Null(proxy.GetType().GetTypeInfo().GetConstructor(new[] {typeof(IInterceptor[]), typeof(bool)}));
		}

		// [Fact] This will be raised as an issue later, it passes in Visual Studio but fails for dotnet test ... might be a tooling problem not code
		public void InternalConstructorIsReplicatedWhenInternalsVisibleTo()
		{
			ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport = false;
			ResetGeneratorAndBuilder();
			try
			{
				var proxy = generator.CreateClassProxy(typeof(InternalClass), new StandardInterceptor());
				Assert.NotNull(proxy.GetType().GetTypeInfo().GetConstructor(new[] {typeof(IInterceptor[])}));
			}
			finally
			{
				ModuleScopeAssemblyNamingOptions.UseAutoNamingConventionsAndDisableFriendAssemblySupport = true;
			}
		}

		[Fact]
		public void ProtectedConstructor()
		{
			var proxy = generator.CreateClassProxy(typeof(NonPublicConstructorClass), new StandardInterceptor()) as NonPublicConstructorClass;
			Assert.NotNull(proxy);
			proxy.DoSomething();
		}

		[Fact]
		public void ProtectedInternalConstructor()
		{
			var proxy = generator.CreateClassProxy(typeof(ProtectedInternalConstructorClass), new StandardInterceptor()) as ProtectedInternalConstructorClass;
			Assert.NotNull(proxy);
			proxy.DoSomething();
		}

		[Fact]
		public void ProtectedMethods()
		{
			var logger = new LogInvocationInterceptor();
			var proxy = (NonPublicMethodsClass) generator.CreateClassProxy(typeof(NonPublicMethodsClass), logger);
			proxy.DoSomething();
			Assert.Equal(2, logger.Invocations.Count);
			Assert.Equal("DoSomething", logger.Invocations[0]);
			Assert.Equal("DoOtherThing", logger.Invocations[1]);
		}
	}
}