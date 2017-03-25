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
using System.Collections;
using System.Reflection;
using Castle.Core.Core;
using Xunit;

namespace Castle.Core.Tests
{
	public class ReflectionBasedDictionaryAdapterTestCase
	{
		public class Customer
		{
			public Customer(int id, string name)
				: this(id, name, false)
			{
			}

			public Customer(int id, string name, bool writeOnly)
			{
				Id = id;
				Name = name;
				IsWriteOnly = writeOnly;
			}

			public int Id { get; set; }

			public string Name { get; set; }

			public bool WriteOnly
			{
				set { IsWriteOnly = value; }
			}

			public bool IsWriteOnly { get; private set; }

			public string this[int id]
			{
				get { return "abcdef"; }
			}
		}

		[Fact]
		public void CanAccessExistingPropertiesInACaseInsensitiveFashion()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.True(dict.Contains("id"));
			Assert.True(dict.Contains("ID"));
			Assert.True(dict.Contains("Id"));
			Assert.True(dict.Contains("name"));
			Assert.True(dict.Contains("Name"));
			Assert.True(dict.Contains("NAME"));
		}

		[Fact]
		public void CanAccessPropertiesValues()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.Equal(1, dict["id"]);
			Assert.Equal("name", dict["name"]);
		}

		[Fact]
		public void CannotCreateWithNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new ReflectionBasedDictionaryAdapter(null)
			);
		}

		[Fact]
		public void EnumeratorIteration()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new {foo = 1, name = "jonh", age = 25});

			Assert.Equal(3, dict.Count);

			var enumerator = (IDictionaryEnumerator) dict.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Assert.NotNull(enumerator.Key);
				Assert.NotNull(enumerator.Value);
			}
		}

		[Fact]
		public void InexistingPropertiesReturnsNull()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.Null(dict["age"]);
		}

		[Fact]
		public void ShouldNotAccessInexistingProperties()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.False(dict.Contains("Age"), "Age property found when it should not be");
			Assert.False(dict.Contains("Address"), "Address property found when it should not be");
		}

		[Fact]
		public void ShouldNotAccessWriteOnlyProperties()
		{
			try
			{
				var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name", true));
				Assert.True((bool) dict["IsWriteOnly"]);
			}
			catch (ArgumentException)
			{
				Assert.False(true, "Attempted to read a write-only property");
			}
		}

		[Fact]
		public void Using_anonymous_types_works_without_exception()
		{
			var target = new {foo = 1, name = "john", age = 25};
			Assert.False(target.GetType().GetTypeInfo().IsPublic);
			var dict = new ReflectionBasedDictionaryAdapter(target);

			Assert.Equal(3, dict.Count);

			Assert.Equal(1, dict["foo"]);
			Assert.Equal("john", dict["name"]);
			Assert.Equal(25, dict["age"]);
		}
	}
}