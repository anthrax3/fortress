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
using Castle.DynamicProxy.Generators;
using Xunit;


namespace Castle.Core.Tests
{
	public class MethodFinderTestCase
	{
		private static void AssertArraysAreEqualUnsorted(object[] expected, object[] actual)
		{
			Assert.Equal(expected.Length, actual.Length);
			var actualAsList = new List<object>(actual);
			foreach (var expectedElement in expected)
			{
				Assert.Contains(expectedElement, actualAsList);
				actualAsList.Remove(expectedElement);
				// need to remove the element after it has been found to guarantee that duplicate elements are handled correctly
			}
		}

		[Fact]
		public void AssertArrayAreEqualUnsorted()
		{
			AssertArraysAreEqualUnsorted(new object[0], new object[0]);
			AssertArraysAreEqualUnsorted(new object[] {null}, new object[] {null});
			AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {null, null, "one"});
			AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", null, null});

			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", "one", null});
				Assert.True(false);
			}
			catch (Exception)
			{
				// ok
			}
			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one"}, new object[] {"one", null, null});
				Assert.True(false);
			}
			catch (Exception)
			{
				// ok
			}
			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", null});
				Assert.True(false);
			}
			catch (Exception)
			{
				// ok
			}
		}

		[Fact]
		public void GetMethodsForNonPublic()
		{
			var methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.NonPublic);
			var realMethods = typeof(object).GetTypeInfo().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Fact]
		public void GetMethodsForPublic()
		{
			var methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.Public);
			var realMethods = typeof(object).GetTypeInfo().GetMethods(BindingFlags.Instance | BindingFlags.Public);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Fact]
		public void GetMethodsForPublicAndNonPublic()
		{
			var methods =
				MethodFinder.GetAllInstanceMethods(typeof(object),
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var realMethods =
				typeof(object).GetTypeInfo().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Fact]
		public void GetMethodsThrowsOnOtherFlags()
		{
			Assert.Throws<ArgumentException>(() =>
				MethodFinder.GetAllInstanceMethods(typeof(object),
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
			);
		}

		[Fact]
		public void GetMethodsThrowsOnStatic()
		{
			Assert.Throws<ArgumentException>(() =>
				MethodFinder.GetAllInstanceMethods(typeof(object),
					BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
			);
		}
	}
}