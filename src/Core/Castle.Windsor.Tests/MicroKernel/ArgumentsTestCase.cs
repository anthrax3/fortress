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
using System.Collections;
using System.Collections.Generic;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Windsor;
using Xunit;

namespace Castle.Windsor.Tests.MicroKernel
{
	
	public class ArgumentsTestCase : AbstractContainerTestCase
	{
		[Fact]
		public void By_default_any_type_as_key_is_supported()
		{
			var arguments = new Arguments(new CustomStringComparer());
			var key = new object();
			var value = "foo";

			arguments.Add(key, value);

			Assert.Equal("foo", arguments[key]);
		}

		[Fact]
		[Bug("IOC-147")]
		public void Can_have_dictionary_as_inline_dependency()
		{
			var container = new WindsorContainer();
			container.Register(Component.For<HasDictionaryDependency>());

			var dictionaryProperty = new Dictionary<string, string>();

			var obj = container.Resolve<HasDictionaryDependency>(new {dictionaryProperty});
			Assert.Same(dictionaryProperty, obj.DictionaryProperty);
		}

		[Fact]
		[Bug("IOC-92")]
		public void Can_mix_hashtable_parameters_and_configuration_parameters()
		{
			Container.Register(
				Component.For<HasStringAndIntDependency>()
					.DependsOn(Parameter.ForKey("x").Eq("abc"))
			);

			Container.Resolve<HasStringAndIntDependency>(new Arguments().Insert("y", 1));
		}

		[Fact]
		[Bug("IOC-142")]
		public void Can_satisfy_nullable_ctor_dependency()
		{
			Container.Register(Component.For<HasNullableDoubleConstructor>());

			var s = Container.Resolve<HasNullableDoubleConstructor>(new Arguments().Insert("foo", 5d));
			Assert.NotNull(s);
		}

		[Fact]
		[Bug("IOC-142")]
		public void Can_satisfy_nullable_property_dependency()
		{
			Container.Register(Component.For<HasNullableIntProperty>());

			var arguments = new Arguments().Insert("SomeVal", 5);
			var s = Container.Resolve<HasNullableIntProperty>(arguments);

			Assert.NotNull(s.SomeVal);
		}

		[Fact]
		public void Custom_stores_get_picked_over_default_ones()
		{
			var arguments = new Arguments(new CustomStringComparer());
			var key = "foo";
			var value = new object();

			arguments.Add(key, value);

			Assert.Equal(value, arguments["boo!"]);
		}

		[Fact]
		public void Handles_string_as_key()
		{
			var arguments = new Arguments();
			var key = "Foo";
			var value = new object();

			arguments.Add(key, value);

			Assert.Equal(1, arguments.Count);
			Assert.True(arguments.Contains(key));
			Assert.Same(value, arguments[key]);
		}

		[Fact]
		public void Handles_string_as_key_case_insensitive()
		{
			var arguments = new Arguments();
			var key = "foo";
			var value = new object();

			arguments.Add(key, value);

			Assert.True(arguments.Contains(key.ToLower()));
			Assert.True(arguments.Contains(key.ToUpper()));
		}

		[Fact]
		public void Handles_Type_as_key()
		{
			var arguments = new Arguments();
			var key = typeof(object);
			var value = new object();

			arguments.Add(key, value);

			Assert.Equal(1, arguments.Count);
			Assert.True(arguments.Contains(key));
			Assert.Same(value, arguments[key]);
		}
	}
}