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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Castle.Core.Internal
{
    public static class AttributesUtil
	{
		private static readonly AttributeUsageAttribute DefaultAttributeUsage = new AttributeUsageAttribute(AttributeTargets.All);

		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return GetAttributes<T>(type).FirstOrDefault();
		}

		public static IEnumerable<T> GetAttributes<T>(this Type type) where T : Attribute
		{
			foreach (T a in type.GetTypeInfo().GetCustomAttributes(typeof(T), false))
				yield return a;
		}

		public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
		{
			return GetAttributes<T>(member).FirstOrDefault();
		}

		public static IEnumerable<T> GetAttributes<T>(this MemberInfo member) where T : Attribute
		{
			foreach (T a in member.GetCustomAttributes(typeof(T), false))
				yield return a;
		}

		public static T GetTypeAttribute<T>(this Type type) where T : Attribute
		{
			var attribute = GetAttribute<T>(type);

			if (attribute == null)
				foreach (var baseInterface in type.GetTypeInfo().GetInterfaces())
				{
					attribute = GetTypeAttribute<T>(baseInterface);
					if (attribute != null)
						break;
				}

			return attribute;
		}

		public static T[] GetTypeAttributes<T>(Type type) where T : Attribute
		{
			var attributes = GetAttributes<T>(type).ToArray();

			if (attributes.Length == 0)
				foreach (var baseInterface in type.GetTypeInfo().GetInterfaces())
				{
					attributes = GetTypeAttributes<T>(baseInterface);
					if (attributes.Length > 0)
						break;
				}

			return attributes;
		}

		public static AttributeUsageAttribute GetAttributeUsage(this Type attributeType)
		{
			var attributes = attributeType.GetTypeInfo().GetCustomAttributes<AttributeUsageAttribute>(true).ToArray();
			return attributes.Length != 0 ? attributes[0] : DefaultAttributeUsage;
		}

		public static Type GetTypeConverter(MemberInfo member)
		{
			var attrib = GetAttribute<TypeConverterAttribute>(member);

			if (attrib != null)
				return Type.GetType(attrib.ConverterTypeName);

			return null;
		}
	}
}