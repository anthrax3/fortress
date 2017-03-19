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
using System.Reflection;
using Castle.Core.DynamicProxy;
using Castle.Core.DynamicProxy.Internal;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class CanDefineAdditionalCustomAttributes : BasePEVerifyTestCase
	{
		[Test]
		public void Can_clone_attributes_with_array_enums()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithEnumArray));
		}

		[Test]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_ints()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithIntArray));
		}

		[Test]
		[Bug("DYNPROXY-151")]
		public void Can_clone_attributes_with_array_types()
		{
			generator.CreateInterfaceProxyWithoutTarget(typeof(IHasAttributeWithTypeArray));
		}

		[Test]
		public void On_class()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateInfo<__Protect>());

			var proxy = generator.CreateClassProxy(typeof(CanDefineAdditionalCustomAttributes), options);

			Assert.IsTrue(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}

		[Test]
		public void On_interfaces()
		{
			var options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateInfo<__Protect>());

			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IDisposable), new Type[0], options);

			Assert.IsTrue(proxy.GetType().GetTypeInfo().IsDefined(typeof(__Protect), false));
		}
	}
}