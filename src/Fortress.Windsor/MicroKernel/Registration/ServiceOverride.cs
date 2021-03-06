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

namespace Castle.MicroKernel.Registration
{
	public class ServiceOverride
	{
		internal ServiceOverride(object dependencyKey, object value) : this(dependencyKey, value, null)
		{
		}

		internal ServiceOverride(object dependencyKey, object value, Type type)
		{
			DependencyKey = dependencyKey;
			Value = value;
			Type = type;
		}

		public object DependencyKey { get; private set; }

		public Type Type { get; private set; }

		public object Value { get; private set; }

		public static ServiceOverrideKey ForKey(string key)
		{
			return new ServiceOverrideKey(key);
		}

		public static ServiceOverrideKey ForKey(Type key)
		{
			return new ServiceOverrideKey(key);
		}

		public static ServiceOverrideKey ForKey<TKey>()
		{
			return new ServiceOverrideKey(typeof(TKey));
		}

		public static implicit operator Dependency(ServiceOverride item)
		{
			return item == null ? null : new Dependency(item);
		}
	}
}