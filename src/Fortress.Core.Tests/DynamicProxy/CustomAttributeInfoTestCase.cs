// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using Xunit;


namespace Castle.Core.Tests.DynamicProxy.Tests
{
	public class CustomAttributeInfoTestCase
	{
		private class MyAttribute1 : Attribute
		{
			public MyAttribute1()
			{
			}

			public MyAttribute1(int intArgument, string stringArgument, int[] arrayArgument)
			{
			}

			public int IntProperty { get; set; }
			public string StringProperty { get; set; }
			public int[] ArrayProperty { get; set; }

#pragma warning disable 649
			public int intField;
			public string stringField;
			public int[] arrayField;
#pragma warning restore 649
		}

		private class MyAttribute2 : Attribute
		{
			public MyAttribute2(int intArgument, string stringArgument, int[] arrayArgument)
			{
			}
		}

		public static IEnumerable<object[]> FromExpressionTestCases()
		{
			var defaultCtor = typeof(MyAttribute1).GetType().GetTypeInfo().GetConstructor(Type.EmptyTypes);
			var ctorWithArgs = typeof(MyAttribute1).GetType().GetTypeInfo().GetConstructor(new[] {typeof(int), typeof(string), typeof(int[])});
			var intProperty = typeof(MyAttribute1).GetType().GetTypeInfo().GetProperty("IntProperty");
			var stringProperty = typeof(MyAttribute1).GetType().GetTypeInfo().GetProperty("StringProperty");
			var arrayProperty = typeof(MyAttribute1).GetType().GetTypeInfo().GetProperty("ArrayProperty");
			var intField = typeof(MyAttribute1).GetType().GetTypeInfo().GetField("intField");
			var stringField = typeof(MyAttribute1).GetType().GetTypeInfo().GetField("stringField");
			var arrayField = typeof(MyAttribute1).GetType().GetTypeInfo().GetField("arrayField");

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1(),
				new CustomAttributeInfo(defaultCtor, new object[0]));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1(42, "foo", new[] {1, 2, 3}),
				new CustomAttributeInfo(ctorWithArgs, new object[] {42, "foo", new[] {1, 2, 3}}));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1 {IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}},
				new CustomAttributeInfo(
					defaultCtor,
					new object[0],
					new[] {intProperty, stringProperty, arrayProperty},
					new object[] {42, "foo", new[] {1, 2, 3}}));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1 {intField = 42, stringField = "foo", arrayField = new[] {1, 2, 3}},
				new CustomAttributeInfo(
					defaultCtor,
					new object[0],
					new[] {intField, stringField, arrayField},
					new object[] {42, "foo", new[] {1, 2, 3}}));
		}

		private static object[] CreateFromExpressionTestCase(Expression<Func<Attribute>> expr, CustomAttributeInfo expected)
		{
			return new object[] {expr, expected};
		}

		[Fact]
		public void Attributes_Of_Different_Type_With_Same_Constructor_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] {1, 2, 3}));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute2(42, "foo", new[] {1, 2, 3}));

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Array_Fields_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 42, stringField = "foo", arrayField = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 99, stringField = "foo", arrayField = new[] {1, 2, 4}});

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Array_Properties_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 99, StringProperty = "foo", ArrayProperty = new[] {1, 2, 4}});

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Constructor_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] {1, 2, 3}));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(99, "foo", new[] {1, 2, 3}));

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Constructor_Array_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] {1, 2, 3}));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(99, "foo", new[] {1, 2, 4}));

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Fields_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 42, stringField = "foo", arrayField = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 99, stringField = "foo", arrayField = new[] {1, 2, 3}});

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Different_Properties_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 99, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}});

			Assert.NotEqual(x, y);
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Same_Constructor_Arguments_Are_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] {1, 2, 3}));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] {1, 2, 3}));

			Assert.Equal(x, y);
			Assert.Equal(x.GetHashCode(), y.GetHashCode());
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Same_Fields_Are_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 42, stringField = "foo", arrayField = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {intField = 42, stringField = "foo", arrayField = new[] {1, 2, 3}});

			Assert.Equal(x, y);
			Assert.Equal(x.GetHashCode(), y.GetHashCode());
		}

		[Fact]
		public void Attributes_Of_Same_Type_With_Same_Properties_Are_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}});
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 {IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] {1, 2, 3}});

			Assert.Equal(x, y);
			Assert.Equal(x.GetHashCode(), y.GetHashCode());
		}

        // Not sure how we deal with these
        //[Fact]
		//[TestCaseSource("FromExpressionTestCases")]
		//public void FromExpression_Creates_Same_CustomAttributeInfo_As_Calling_The_Constructor(
		//	Expression<Func<Attribute>> expr, CustomAttributeInfo expected)
		//{
		//	var actual = CustomAttributeInfo.FromExpression(expr);
		//	Assert.Equal(expected, actual);
		//}
	}
}