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

using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.DynamicProxy.Tests.Interfaces;
using Castle.Core.Tests.Interceptors;
using Xunit;


namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class ClassProxyWithDefaultValuesTestCase : CoreBaseTestCase
	{
		[Fact]
		public void MethodParameterWithDefaultValue_UseNullDefaultValue_class_proxy()
		{
			var proxy = generator.CreateClassProxy<ClassWithMethodWithParameterWithNullDefaultValue>();
			var result = proxy.Method();

			Assert.True(result);
		}

		[Fact]
		public void MethodParameterWithDefaultValue_UseNullDefaultValue_interface_proxy()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<InterfaceWithMethodWithParameterWithNullDefaultValue>(
				new SetReturnValueInterceptor(true));
			var result = proxy.Method();

			Assert.True(result);
		}
	}
}