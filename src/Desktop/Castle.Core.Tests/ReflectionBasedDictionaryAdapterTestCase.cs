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
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
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

		[Test]
		public void CanAccessExistingPropertiesInACaseInsensitiveFashion()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsTrue(dict.Contains("id"));
			Assert.IsTrue(dict.Contains("ID"));
			Assert.IsTrue(dict.Contains("Id"));
			Assert.IsTrue(dict.Contains("name"));
			Assert.IsTrue(dict.Contains("Name"));
			Assert.IsTrue(dict.Contains("NAME"));
		}

		[Test]
		public void CanAccessPropertiesValues()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.AreEqual(1, dict["id"]);
			Assert.AreEqual("name", dict["name"]);
		}

		[Test]
		public void CannotCreateWithNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new ReflectionBasedDictionaryAdapter(null)
			);
		}

		[Test]
		public void EnumeratorIteration()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new {foo = 1, name = "jonh", age = 25});

			Assert.AreEqual(3, dict.Count);

			var enumerator = (IDictionaryEnumerator) dict.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Assert.IsNotNull(enumerator.Key);
				Assert.IsNotNull(enumerator.Value);
			}
		}

		[Test]
		public void InexistingPropertiesReturnsNull()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsNull(dict["age"]);
		}

		[Test]
		public void ShouldNotAccessInexistingProperties()
		{
			var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name"));

			Assert.IsFalse(dict.Contains("Age"), "Age property found when it should not be");
			Assert.IsFalse(dict.Contains("Address"), "Address property found when it should not be");
		}

		[Test /*(Description = "Test case for patch supplied on the mailing list by Jan Limpens")*/]
		public void ShouldNotAccessWriteOnlyProperties()
		{
			try
			{
				var dict = new ReflectionBasedDictionaryAdapter(new Customer(1, "name", true));
				Assert.IsTrue((bool) dict["IsWriteOnly"]);
			}
			catch (ArgumentException)
			{
				Assert.Fail("Attempted to read a write-only property");
			}
		}

		[Test]
		public void Using_anonymous_types_works_without_exception()
		{
			var target = new {foo = 1, name = "john", age = 25};
			Assert.IsFalse(target.GetType().GetTypeInfo().IsPublic);
			var dict = new ReflectionBasedDictionaryAdapter(target);

			Assert.AreEqual(3, dict.Count);

			Assert.AreEqual(1, dict["foo"]);
			Assert.AreEqual("john", dict["name"]);
			Assert.AreEqual(25, dict["age"]);
		}
	}
}