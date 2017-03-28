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

using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests
{
	public class ClassWithAttributesTestCase : CoreBaseTestCase
	{
		[Fact]
		public void Can_proxy_type_with_non_inheritable_attribute_depending_on_array_of_something_via_field()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHasNonInheritableAttributeWithArray2>();
			var attribute = proxy.GetType().GetTypeInfo()
				.GetCustomAttributes(typeof(NonInheritableWithArray2Attribute), false)
				.Cast<NonInheritableWithArray2Attribute>().Single();
			Assert.Equal(attribute.Values, new[] {"1", "2", "3"});
		}

		[Fact]
		public void Can_proxy_type_with_non_inheritable_attribute_depending_on_array_of_something_via_property()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IHasNonInheritableAttributeWithArray>();
			var attribute = proxy.GetType().GetTypeInfo()
				.GetCustomAttributes(typeof(NonInheritableWithArrayAttribute), false)
				.Cast<NonInheritableWithArrayAttribute>().Single();
			Assert.Equal(attribute.Values, new[] {"1", "2", "3"});
		}

		[Fact]
		public void EnsureProxyHasAttributesOnClassAndMethods()
		{
			var instance = (HasNonInheritableAttribute)
				generator.CreateClassProxy(typeof(HasNonInheritableAttribute), new StandardInterceptor());

			var attributes = instance.GetType().GetTypeInfo().GetCustomAttributes(typeof(NonInheritableAttribute), false).ToArray();
			Assert.Equal(1, attributes.Length);
			Assert.IsAssignableFrom(typeof(NonInheritableAttribute), attributes[0]);

			attributes = instance.GetType().GetTypeInfo().GetMethod("OnMethod").GetCustomAttributes(typeof(NonInheritableAttribute), false).ToArray();
			Assert.Equal(1, attributes.Length);
			Assert.IsAssignableFrom(typeof(NonInheritableAttribute), attributes[0]);
		}

		[Fact]
		public void EnsureProxyHasAttributesOnClassAndMethods_ComplexAttributes()
		{
			var instance = (AttributedClass2)
				generator.CreateClassProxy(typeof(AttributedClass2), new StandardInterceptor());

			var attributes = instance.GetType().GetTypeInfo().GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false).ToArray();
			Assert.Equal(1, attributes.Length);
			Assert.IsAssignableFrom(typeof(ComplexNonInheritableAttribute), attributes[0]);
			var att = (ComplexNonInheritableAttribute) attributes[0];
			// (1, 2, true, "class", FileAccess.Write)
			Assert.Equal(1, att.Id);
			Assert.Equal(2, att.Num);
			Assert.Equal(true, att.IsSomething);
			Assert.Equal("class", att.Name);
			Assert.Equal(FileAccess.Write, att.Access);

			attributes = instance.GetType().GetTypeInfo().GetMethod("Do1").GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false).ToArray();
			Assert.Equal(1, attributes.Length);
			Assert.IsAssignableFrom(typeof(ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (2, 3, "Do1", Access = FileAccess.ReadWrite)
			Assert.Equal(2, att.Id);
			Assert.Equal(3, att.Num);
			Assert.Equal(false, att.IsSomething);
			Assert.Equal("Do1", att.Name);
			Assert.Equal(FileAccess.ReadWrite, att.Access);

			attributes = instance.GetType().GetTypeInfo().GetMethod("Do2").GetCustomAttributes(typeof(ComplexNonInheritableAttribute), false).ToArray();
			Assert.Equal(1, attributes.Length);
			Assert.IsAssignableFrom(typeof(ComplexNonInheritableAttribute), attributes[0]);
			att = (ComplexNonInheritableAttribute) attributes[0];
			// (3, 4, "Do2", IsSomething=true)
			Assert.Equal(3, att.Id);
			Assert.Equal(4, att.Num);
			Assert.Equal(true, att.IsSomething);
			Assert.Equal("Do2", att.Name);
		}

		[Fact]
		public void EnsureProxyHasAttributesOnParameter()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			var nameProperty = proxy.GetType().GetTypeInfo().GetMethod("OnParameter").GetParameters().Single();
			Assert.True(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}

		[Fact]
		public void EnsureProxyHasAttributesOnProperties()
		{
			var proxy = generator.CreateClassProxy<HasNonInheritableAttribute>();
			var nameProperty = proxy.GetType().GetTypeInfo().GetProperty("OnProperty");
			Assert.True(nameProperty.IsDefined(typeof(NonInheritableAttribute), false));
		}
	}
}