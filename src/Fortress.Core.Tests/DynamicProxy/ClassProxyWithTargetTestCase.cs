// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Tests.DynamicProxy.Tests.Classes;
using Castle.Core.Tests.Mixins;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Xunit;


namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class ClassProxyWithTargetTestCase : CoreBaseTestCase
	{
		private class PrivateClass
		{
		}

		[Fact]
		public void Can_proxy_class_with_no_default_ctor()
		{
			var proxy = generator.CreateClassProxyWithTarget(typeof(VirtualClassWithNoDefaultCtor),
				new VirtualClassWithNoDefaultCtor(42),
				new object[] {12});
			var result = ((VirtualClassWithNoDefaultCtor) proxy).Method();
			Assert.Equal(42, result);
		}

		[Fact]
		[Bug("DYNPROXY-170")]
		public void Can_proxy_class_with_protected_generic_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassWithProtectedGenericMethod(42));
			var result = proxy.PublicMethod<int>();
			Assert.Equal(42, result);
		}

		[Fact]
		[Bug("DYNPROXY-170")]
		public void Can_proxy_class_with_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassWithProtectedMethod(42));
			var result = proxy.PublicMethod();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_class_with_two_protected_methods_differing_by_return_type()
		{
			generator.CreateClassProxyWithTarget(new HasTwoProtectedMethods());
		}

		[Fact]
		public void Can_proxy_generic_class()
		{
			generator.CreateClassProxyWithTarget(new List<object>());
		}

		[Fact]
		public void Can_proxy_purely_virtual_class()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithMethod());
			var result = proxy.Method();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_purely_virtual_inherited_abstract_class()
		{
			var proxy = generator.CreateClassProxyWithTarget<AbstractClassWithMethod>(new InheritsAbstractClassWithMethod());
			var result = proxy.Method();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_virtual_class_with_protected_generic_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedGenericMethod(42));
			var result = proxy.PublicMethod<int>();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_virtual_class_with_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new VirtualClassWithProtectedMethod(42));
			var result = proxy.PublicMethod();
			Assert.Equal(42, result);
		}

		[Fact]
		public void Can_proxy_with_target_after_proxy_without_target_for_the_same_type()
		{
			generator.CreateClassProxy<SimpleClass>();

			generator.CreateClassProxyWithTarget(new SimpleClass());
		}

		[Fact]
		public void Cannot_proxy_generic_class_with_inaccessible_type_argument()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateClassProxyWithTarget(new List<PrivateClass>()));
		}

		[Fact]
		public void Cannot_proxy_generic_class_with_type_argument_that_has_inaccessible_type_argument()
		{
			var ex = Assert.Throws<GeneratorException>(() => generator.CreateClassProxyWithTarget(new List<List<PrivateClass>>()));

			var expected = string.Format("Can not create proxy for type {0} because type {1} is not accessible. Make it public, or internal",
				typeof(List<List<PrivateClass>>).FullName, typeof(PrivateClass).FullName);
		}

		[Fact]
		public void Cannot_proxy_inaccessible_class()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateClassProxyWithTarget(new PrivateClass()));
		}

		[Fact]
		public void Hook_does_NOT_get_notified_about_autoproperty_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithAutoProperty), false);

			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithAutoProperty), Type.EmptyTypes,
				new VirtualClassWithAutoProperty(), new ProxyGenerationOptions(hook),
				new object[0]);

			Assert.False(hook.NonVirtualMembers.Any(m => m is FieldInfo));
		}

		[Fact]
		public void Hook_gets_notified_about_public_field()
		{
			var hook = new LogHook(typeof(VirtualClassWithPublicField), false);
			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithPublicField), Type.EmptyTypes,
				new VirtualClassWithPublicField(), new ProxyGenerationOptions(hook),
				new object[0]);
			Assert.NotEmpty((ICollection) hook.NonVirtualMembers);
			var memberInfo = hook.NonVirtualMembers.Single(m => m is FieldInfo);
			Assert.Equal("field", memberInfo.Name);
			Assert.Equal(MemberTypes.Field, memberInfo.MemberType);
		}

		[Fact]
		public void Hook_gets_notified_about_static_methods()
		{
			var hook = new LogHook(typeof(VirtualClassWithPublicField), false);
			generator.CreateClassProxyWithTarget(typeof(VirtualClassWithPublicField), Type.EmptyTypes,
				new VirtualClassWithPublicField(), new ProxyGenerationOptions(hook),
				new object[0]);
			Assert.NotEmpty((ICollection) hook.NonVirtualMembers);
			var memberInfo = hook.NonVirtualMembers.Single(m => m is FieldInfo);
			Assert.Equal("field", memberInfo.Name);
			Assert.Equal(MemberTypes.Field, memberInfo.MemberType);
		}

		[Fact]
		[Bug("DYNPROXY-185")]
		public void Returns_proxy_target_instead_of_self()
		{
			var target = new Classes.EmptyClass();
			var proxy = generator.CreateClassProxyWithTarget(target);
			var result = (Classes.EmptyClass) ((IProxyTargetAccessor) proxy).DynProxyGetTarget();
			Assert.Equal(target, result);
		}

		[Fact]
		public void Uses_The_Provided_Options()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SimpleMixin());

			var target = new SimpleClassWithProperty();
			var proxy = generator.CreateClassProxyWithTarget(target, options);

			Assert.IsAssignableFrom(typeof(ISimpleMixin), proxy);
		}
	}
}