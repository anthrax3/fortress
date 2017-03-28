// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.InterClasses;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class GenerationHookTestCase : CoreBaseTestCase
	{
		[Fact]
		public void Hook_can_NOT_see_GetType_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var getType = typeof(EmptyClass).GetTypeInfo().GetMethod("GetType");
			Assert.DoesNotContain(getType, hook.AskedMembers);
			Assert.DoesNotContain(getType, hook.NonVirtualMembers);
		}

		[Fact]
		public void Hook_can_NOT_see_MemberwiseClone_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var memberwiseClone = typeof(EmptyClass).GetTypeInfo().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.DoesNotContain(memberwiseClone, hook.AskedMembers);
			Assert.DoesNotContain(memberwiseClone, hook.NonVirtualMembers);
		}

		[Fact]
		public void Hook_can_see_Equals_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var equals = typeof(EmptyClass).GetTypeInfo().GetMethod("Equals");
			Assert.Contains(equals, hook.AskedMembers);
		}

		[Fact]
		public void Hook_can_see_GetHashCode_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var getHashCode = typeof(EmptyClass).GetTypeInfo().GetMethod("GetHashCode");
			Assert.Contains(getHashCode, hook.AskedMembers);
		}

		[Fact]
		public void Hook_can_see_ToString_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var equals = typeof(EmptyClass).GetTypeInfo().GetMethod("ToString");
			Assert.Contains(equals, hook.AskedMembers);
		}

		[Fact]
		public void HookDetectsNonVirtualAlthoughInterfaceImplementation()
		{
			var logger = new LogInvocationInterceptor();
			var hook = new LogHook(typeof(ServiceImpl), true);

			var options = new ProxyGenerationOptions(hook);

			// we are creating a class proxy although the creation of an interface proxy is possible too...
			// since the members of our implementation are not explicitly marked as virtual, the runtime
			// marks them as virtual but final --> not good for us, but intended by .net :-(
			//
			// see: https://msdn.microsoft.com/library/system.reflection.methodbase.isvirtual
			//
			// thus, a non virtual notification for this particular situation is appropriate
			generator.CreateClassProxy(typeof(ServiceImpl), options, logger);

			Assert.True(hook.Completed);
			Assert.Equal(3, hook.AskedMembers.Count);
			Assert.Equal(11, hook.NonVirtualMembers.Count);
		}

		[Fact]
		public void HookIsUsedForConcreteClassProxy()
		{
			var logger = new LogInvocationInterceptor();
			var hook = new LogHook(typeof(ServiceClass), true);

			var options = new ProxyGenerationOptions(hook);

			var proxy = (ServiceClass) generator.CreateClassProxy(typeof(ServiceClass), options, logger);

			Assert.True(hook.Completed);
			Assert.Equal(13, hook.AskedMembers.Count);
			Assert.Equal(2, hook.NonVirtualMembers.Count);

			proxy.Sum(1, 2);
			Assert.False(proxy.Valid);

			Assert.Equal("get_Valid ", logger.LogContents);
		}

		[Fact]
		public void HookIsUsedForInterfaceProxy()
		{
			var logger = new LogInvocationInterceptor();
			var hook = new LogHook(typeof(IService), false);

			var options = new ProxyGenerationOptions(hook);

			var proxy = (IService)
				generator.CreateInterfaceProxyWithTarget(
					typeof(IService), new ServiceImpl(), options, logger);

			Assert.True(hook.Completed);
			Assert.Equal(10, hook.AskedMembers.Count);
			Assert.Equal(0, hook.NonVirtualMembers.Count);

			Assert.Equal(3, proxy.Sum(1, 2));
			Assert.False(proxy.Valid);

			Assert.Equal("Sum get_Valid ", logger.LogContents);
		}
	}
}