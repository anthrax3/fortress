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
using System.Reflection;

namespace Castle.DynamicProxy.Generators
{
	public class CacheKey
	{
		private readonly Type[] interfaces;
		private readonly ProxyGenerationOptions options;
		private readonly MemberInfo target;
		private readonly Type type;

		public CacheKey(MemberInfo target, Type type, Type[] interfaces, ProxyGenerationOptions options)
		{
			this.target = target;
			this.type = type;
			this.interfaces = interfaces ?? Type.EmptyTypes;
			this.options = options;
		}

		public CacheKey(Type target, Type[] interfaces, ProxyGenerationOptions options)
			: this(target.GetTypeInfo(), null, interfaces, options)
		{
		}

		public override int GetHashCode()
		{
			var result = target.GetHashCode();
			foreach (var inter in interfaces)
				result += 29 + inter.GetHashCode();
			if (options != null)
				result = 29 * result + options.GetHashCode();
			if (type != null)
				result = 29 * result + type.GetHashCode();
			return result;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			var cacheKey = obj as CacheKey;
			if (cacheKey == null)
				return false;

			if (!Equals(type, cacheKey.type))
				return false;
			if (!Equals(target, cacheKey.target))
				return false;
			if (interfaces.Length != cacheKey.interfaces.Length)
				return false;
			for (var i = 0; i < interfaces.Length; i++)
				if (!Equals(interfaces[i], cacheKey.interfaces[i]))
					return false;
			if (!Equals(options, cacheKey.options))
				return false;
			return true;
		}
	}
}