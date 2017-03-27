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
using System.Reflection;
using Castle.Core.Tests.Mixins;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class ProxyGenerationOptionsTestCase
	{
		public ProxyGenerationOptionsTestCase()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();
		}

		private ProxyGenerationOptions _options1;
		private ProxyGenerationOptions _options2;

		[Fact]
		public void Equals_Compares_selectors_existence()
		{
			_options1.Selector = new AllInterceptorSelector();
			_options2.Selector = new TypeInterceptorSelector<StandardInterceptor>();

			Assert.Equal(_options1, _options2);

			_options2.Selector = null;
			Assert.NotEqual(_options1, _options2);

			_options1.Selector = null;
			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_ComparesMixinTypesNotInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_ComparesSortedMixinTypes()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.AddMixinInstance(new ComplexMixin());

			_options2.AddMixinInstance(new ComplexMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentAdditionalAttributes()
		{
			var info1 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});

			_options1.AdditionalAttributes.Add(info1);
			_options2.AdditionalAttributes.Add(info2);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentAdditionalAttributesDuplicateEntries()
		{
			var info11 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info12 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info13 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});

			_options1.AdditionalAttributes.Add(info11);
			_options1.AdditionalAttributes.Add(info12);

			_options2.AdditionalAttributes.Add(info13);
			_options2.AdditionalAttributes.Add(info2);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_AddMixinInstance()
		{
			var mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(object);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_Hook()
		{
			IProxyGenerationHook hook = new LogHook(typeof(object), true);
			_options1.Hook = hook;

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_EmptyOptions()
		{
			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(IConvertible);

			var mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);

			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_SameAdditionalAttributes()
		{
			var info1 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test"});

			_options1.AdditionalAttributes.Add(info1);
			_options2.AdditionalAttributes.Add(info2);

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_SameAdditionalAttributesDifferentOrder()
		{
			var info11 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info12 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info21 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});
			var info22 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});

			_options1.AdditionalAttributes.Add(info11);
			_options1.AdditionalAttributes.Add(info21);

			_options2.AdditionalAttributes.Add(info22);
			_options2.AdditionalAttributes.Add(info12);

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void GetHashCode_DifferentOptions_AddMixinInstance()
		{
			var mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(object);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_DifferentAdditionalAttributes()
		{
			var info1 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});

			_options1.AdditionalAttributes.Add(info1);
			_options2.AdditionalAttributes.Add(info2);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_Hook()
		{
			IProxyGenerationHook hook = new LogHook(typeof(object), true);
			_options1.Hook = hook;

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_SameAdditionalAttributesButDuplicateEntries()
		{
			var info1 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info3 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info4 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});

			_options1.AdditionalAttributes.Add(info1);

			_options2.AdditionalAttributes.Add(info2);
			_options2.AdditionalAttributes.Add(info3);
			_options2.AdditionalAttributes.Add(info4);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EmptyOptions()
		{
			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(IConvertible);

			var mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);


			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualOptions_DifferentMixinInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualOptions_SameAdditionalAttributes()
		{
			var info1 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test"});
			var info2 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test"});
			_options1.AdditionalAttributes.Add(info1);
			_options2.AdditionalAttributes.Add(info2);

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualOptions_SameAdditionalAttributesDifferentOrder()
		{
			var info11 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info12 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test1"});
			var info21 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});
			var info22 = new CustomAttributeInfo(typeof(DescriptionAttribute).GetTypeInfo().GetConstructor(new[] {typeof(string)}), new object[] {"Test2"});


			_options1.AdditionalAttributes.Add(info11);
			_options1.AdditionalAttributes.Add(info21);

			_options2.AdditionalAttributes.Add(info22);
			_options2.AdditionalAttributes.Add(info12);

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void MixinData()
		{
			_options1.Initialize();
			var data = _options1.MixinData;
			Assert.Equal(0, new List<object>(data.Mixins).Count);
		}

		[Fact]
		public void MixinData_NeedsInitialize()
		{
			Assert.Throws<InvalidOperationException>(delegate
			{
#pragma warning disable 219
				var data = _options1.MixinData;
#pragma warning restore 219
			});
		}

		[Fact]
		public void MixinData_NoReInitializeWhenNothingChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			var data1 = _options1.MixinData;
			_options1.Initialize();
			var data2 = _options1.MixinData;
			Assert.Same(data1, data2);
		}

		[Fact]
		public void MixinData_ReInitializeWhenMixinsChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			var data1 = _options1.MixinData;

			_options1.AddMixinInstance(new OtherMixin());
			_options1.Initialize();
			var data2 = _options1.MixinData;
			Assert.NotSame(data1, data2);

			Assert.Equal(1, new List<object>(data1.Mixins).Count);
			Assert.Equal(2, new List<object>(data2.Mixins).Count);
		}

		[Fact]
		public void MixinData_WithMixins()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();
			var data = _options1.MixinData;
			Assert.Equal(1, new List<object>(data.Mixins).Count);
		}
	}
}