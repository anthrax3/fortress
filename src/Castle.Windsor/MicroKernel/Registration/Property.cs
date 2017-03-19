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
using Castle.Windsor.Core;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class Property
	{
		private readonly object key;
		private readonly object value;

		public Property(object key, object value)
		{
			this.key = key;
			this.value = value;
		}

		public object Key
		{
			get { return key; }
		}

		public object Value
		{
			get { return value; }
		}

		public static PropertyKey ForKey(String key)
		{
			return new PropertyKey(key);
		}

		public static PropertyKey ForKey(Type key)
		{
			return new PropertyKey(key);
		}

		public static PropertyKey ForKey<TKey>()
		{
			return new PropertyKey(typeof(TKey));
		}

		public static implicit operator Dependency(Property item)
		{
			return item == null ? null : new Dependency(item);
		}
	}

	public class PropertyKey
	{
		private readonly object key;

		internal PropertyKey(object key)
		{
			this.key = key;
		}

		public object Key
		{
			get { return key; }
		}

		public Property Eq(Object value)
		{
			return new Property(key, value);
		}

		public ServiceOverride Is(string componentName)
		{
			return GetServiceOverrideKey().Eq(componentName);
		}

		public ServiceOverride Is(Type componentImplementation)
		{
			if (componentImplementation == null)
			{
				throw new ArgumentNullException("componentImplementation");
			}
			return GetServiceOverrideKey().Eq(ComponentName.DefaultNameFor(componentImplementation));
		}

		public ServiceOverride Is<TComponentImplementation>()
		{
			return Is(typeof(TComponentImplementation));
		}

		private ServiceOverrideKey GetServiceOverrideKey()
		{
			if (key is Type)
			{
				return ServiceOverride.ForKey((Type)key);
			}
			return ServiceOverride.ForKey((string)key);
		}
	}
}
