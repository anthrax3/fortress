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
using Castle.Core.Tests.Interceptors;
using Castle.Core.Tests.Interfaces;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class ProxyTypeCachingWithDifferentHooksTestCase : CoreBaseTestCase
	{
		public class CustomHook : AllMethodsHook
		{
		}

		private object CreateProxyWithHook<THook>() where THook : IProxyGenerationHook, new()
		{
			return generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), new ProxyGenerationOptions(new THook()), new DoNothingInterceptor());
		}

		[Fact]
		public void Proxies_with_different_hooks_will_use_different_proxy_types()
		{
			var first = CreateProxyWithHook<AllMethodsHook>();
			var second = CreateProxyWithHook<CustomHook>();
			Assert.NotEqual(first.GetType(), second.GetType());
		}

		[Fact]
		public void Proxies_with_same_hook_will_use_cached_proxy_type()
		{
			var first = CreateProxyWithHook<CustomHook>();
			var second = CreateProxyWithHook<CustomHook>();
			Assert.Equal(first.GetType(), second.GetType());
		}
	}
}