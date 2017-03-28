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
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Xunit;


namespace Castle.Core.Tests
{
	public class CacheKeyTestCase
	{
		[Fact]
		public void DifferentKeys()
		{
			var key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			var key2 = new CacheKey(typeof(NonPublicMethodsClass), null, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new[] {typeof(IDisposable)}, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof(NonPublicConstructorClass), new[] {typeof(IConvertible)}, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new[] {typeof(IDisposable)}, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof(NonPublicMethodsClass), new[] {typeof(IDisposable)}, ProxyGenerationOptions.Default);

			Assert.NotEqual(key1, key2);
		}

		[Fact]
		public void DifferentOptions()
		{
			var options1 = new ProxyGenerationOptions();
			var options2 = new ProxyGenerationOptions();
			options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			var key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.NotEqual(key1, key2);

			options1 = new ProxyGenerationOptions();
			options2 = new ProxyGenerationOptions();
			options2.Selector = new AllInterceptorSelector();
			key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.NotEqual(key1, key2);
		}

		[Fact]
		public void EqualNullAndEmptyInterfaces()
		{
			var key1 = new CacheKey(typeof(NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass).GetTypeInfo(), null, Type.EmptyTypes,
				ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);
			Assert.Equal(key2, key1);
		}

		[Fact]
		public void EqualWithProxyForType()
		{
			var key1 = new CacheKey(typeof(NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass).GetTypeInfo(), null, null, ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);

			var key3 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.NotEqual(key1, key3);
			Assert.NotEqual(key3, key1);

			var key4 = new CacheKey(null, null, null, ProxyGenerationOptions.Default);
			Assert.Equal(key4, key3);
			Assert.Equal(key3, key4);
		}

		[Fact]
		public void EquivalentOptions()
		{
			var options1 = new ProxyGenerationOptions();
			var options2 = new ProxyGenerationOptions();

			var key1 = new CacheKey(typeof(NonPublicConstructorClass), null, options1);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass), null, options2);

			Assert.Equal(key1, key2);
		}

		[Fact]
		public void InstanceEquivalence()
		{
			var key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);

			key1 = new CacheKey(typeof(NonPublicConstructorClass), null, ProxyGenerationOptions.Default);
			key2 = new CacheKey(typeof(NonPublicConstructorClass), null, new ProxyGenerationOptions());

			Assert.Equal(key1, key2);
		}

		[Fact]
		public void InstanceEquivalence_WithInterfaces()
		{
			var key1 = new CacheKey(typeof(NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);
			var key2 = new CacheKey(typeof(NonPublicConstructorClass), new Type[0], ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);

			key1 =
				new CacheKey(typeof(NonPublicConstructorClass), new[] {typeof(IDisposable)}, ProxyGenerationOptions.Default);
			key2 =
				new CacheKey(typeof(NonPublicConstructorClass), new[] {typeof(IDisposable)}, ProxyGenerationOptions.Default);

			Assert.Equal(key1, key2);
		}
	}
}