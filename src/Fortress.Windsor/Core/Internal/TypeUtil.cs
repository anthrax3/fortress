// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Castle.Core.Internal
{
	public static class TypeUtil
	{
		public static bool IsPrimitiveTypeOrCollection(this Type type)
		{
			if (type.IsPrimitiveType())
				return true;

			var itemType = type.GetCompatibleArrayItemType();
			return itemType != null && itemType.IsPrimitiveTypeOrCollection();
		}

		public static bool IsPrimitiveType(this Type type)
		{
			return type == null || type.GetTypeInfo().IsValueType || type == typeof(string);
		}

		public static string ToCSharpString(this Type type)
		{
			try
			{
				var name = new StringBuilder();
				ToCSharpString(type, name);
				return name.ToString();
			}
			catch (Exception)
			{
				// in case we messed up something...
				return type.Name;
			}
		}

		[DebuggerHidden]
		public static Type TryMakeGenericType(this Type openGeneric, Type[] arguments)
		{
			try
			{
				return openGeneric.MakeGenericType(arguments);
			}
			catch (ArgumentException)
			{
				// Any element of typeArguments does not satisfy the constraints specified for the corresponding type parameter of the current generic type.
				// NOTE: We try and catch because there's no public API to reliably, and robustly test for that upfront
				// there's RuntimeTypeHandle.SatisfiesConstraints method but it's internal. 
				return null;
			}
		}

		private static void AppendGenericParameters(StringBuilder name, Type[] genericArguments)
		{
			name.Append("<");

			for (var i = 0; i < genericArguments.Length - 1; i++)
			{
				ToCSharpString(genericArguments[i], name);
				name.Append(", ");
			}
			if (genericArguments.Length > 0)
				ToCSharpString(genericArguments[genericArguments.Length - 1], name);
			name.Append(">");
		}

		private static void ToCSharpString(Type type, StringBuilder name)
		{
			if (type.IsArray)
			{
				var elementType = type.GetElementType();
				ToCSharpString(elementType, name);
				name.Append(type.Name.Substring(elementType.Name.Length));
				return;
			}
			if (type.IsGenericParameter)
			{
				//NOTE: this has to go before type.IsNested because nested generic type is also a generic parameter and otherwise we'd have stack overflow
				name.AppendFormat("�{0}�", type.Name);
				return;
			}
			if (type.IsNested)
			{
				ToCSharpString(type.DeclaringType, name);
				name.Append(".");
			}
			if (type.GetTypeInfo().IsGenericType == false)
			{
				name.Append(type.Name);
				return;
			}
			name.Append(type.Name.Split('`')[0]);
			AppendGenericParameters(name, type.GetGenericArguments());
		}
	}
}