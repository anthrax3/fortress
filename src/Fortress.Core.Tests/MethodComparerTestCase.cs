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
	public class MethodComparerTestCase
	{
		public static void GenericMethod<T>()
		{
		}

		public static void GenericMethod2<T>()
		{
		}

		public static void GenericMethod3<T, H>(T t, H h)
		{
		}

		private class FakeScope
		{
			public static void GenericMethod()
			{
			}

			public static void GenericMethod3<T>(T t)
			{
			}
		}

		private class NewScope
		{
			public static void GenericMethod3<T, H>(H t, T h)
			{
			}
		}

		[Fact]
		public void CompareMethods()
		{
			var mc = MethodSignatureComparer.Instance;
			Assert.True(mc.Equals(null, null));
			Assert.False(mc.Equals(null, typeof(object).GetTypeInfo().GetMethod("ToString")));

			Assert.True(mc.Equals(typeof(object).GetTypeInfo().GetMethod("ToString"), typeof(object).GetTypeInfo().GetMethod("ToString")));
			Assert.True(mc.Equals(typeof(List<>).GetTypeInfo().GetMethod("get_Count"), typeof(List<>).GetTypeInfo().GetMethod("get_Count")));
			Assert.True(mc.Equals(typeof(List<int>).GetTypeInfo().GetMethod("get_Count"), typeof(List<int>).GetTypeInfo().GetMethod("get_Count")));
			Assert.True(mc.Equals(typeof(List<>).GetTypeInfo().GetMethod("get_Count"), typeof(List<int>).GetTypeInfo().GetMethod("get_Count")));
			Assert.True(mc.Equals(typeof(List<string>).GetTypeInfo().GetMethod("get_Count"), typeof(List<>).GetTypeInfo().GetMethod("get_Count")));
			Assert.True(mc.Equals(typeof(List<>).GetTypeInfo().GetMethod("get_Item"), typeof(List<>).GetTypeInfo().GetMethod("get_Item")));

			Assert.True(mc.Equals(typeof(List<string>).GetTypeInfo().GetMethod("Add"), typeof(List<string>).GetTypeInfo().GetMethod("Add")));
			Assert.False(mc.Equals(typeof(List<string>).GetTypeInfo().GetMethod("Add"), typeof(List<>).GetTypeInfo().GetMethod("Add")));
			Assert.False(mc.Equals(typeof(List<>).GetTypeInfo().GetMethod("Add"), typeof(List<string>).GetTypeInfo().GetMethod("Add")));

			Assert.True(mc.Equals(typeof(List<string>).GetTypeInfo().GetMethod("get_Item"), typeof(List<string>).GetTypeInfo().GetMethod("get_Item")));
			Assert.False(mc.Equals(typeof(List<string>).GetTypeInfo().GetMethod("get_Item"), typeof(List<>).GetTypeInfo().GetMethod("get_Item")));
			Assert.False(mc.Equals(typeof(List<>).GetTypeInfo().GetMethod("get_Item"), typeof(List<string>).GetTypeInfo().GetMethod("get_Item")));

			Assert.True(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod"),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod")));
			Assert.True(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod").MakeGenericMethod(typeof(int)),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod").MakeGenericMethod(typeof(int))));
			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod").MakeGenericMethod(typeof(int)),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod").MakeGenericMethod(typeof(string))));
			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod").MakeGenericMethod(typeof(int)),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod")));
			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod"),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod2")));

			Assert.True(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3"),
				typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3")));
			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3"),
				typeof(NewScope).GetTypeInfo().GetMethod("GenericMethod3")));
			Assert.True(
				mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(int), typeof(int)),
					typeof(NewScope).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(int), typeof(int))));

			Assert.False(
				mc.Equals(
					typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(int), typeof(string)),
					typeof(NewScope).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(int), typeof(string))));

			Assert.False(
				mc.Equals(
					typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(int), typeof(string)),
					typeof(NewScope).GetTypeInfo().GetMethod("GenericMethod3").MakeGenericMethod(typeof(string), typeof(int))));

			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod3"),
				typeof(FakeScope).GetTypeInfo().GetMethod("GenericMethod3")));

			Assert.False(mc.Equals(typeof(MethodComparerTestCase).GetTypeInfo().GetMethod("GenericMethod"),
				typeof(FakeScope).GetTypeInfo().GetMethod("GenericMethod")));

			Assert.False(mc.Equals(typeof(Console).GetTypeInfo().GetMethod("WriteLine", new[] {typeof(object)}),
				typeof(Console).GetTypeInfo().GetMethod("WriteLine", new[] {typeof(string), typeof(object[])})));
			Assert.True(mc.Equals(typeof(Console).GetTypeInfo().GetMethod("WriteLine", new[] {typeof(string), typeof(object[])}),
				typeof(Console).GetTypeInfo().GetMethod("WriteLine", new[] {typeof(string), typeof(object[])})));
		}
	}
}