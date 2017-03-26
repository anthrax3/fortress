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
using System.Collections;
using System.Collections.Generic;
using Castle.Core.Core.Configuration;

namespace Castle.Windsor.MicroKernel.Registration
{
	public sealed class Dependency
	{
		private readonly object item;

		internal Dependency(object item)
		{
			this.item = item;
		}

		public static Parameter OnConfigValue(string dependencyName, string valueAsString)
		{
			return Parameter.ForKey(dependencyName).Eq(valueAsString);
		}

		public static Parameter OnConfigValue(string dependencyName, IConfiguration value)
		{
			return Parameter.ForKey(dependencyName).Eq(value);
		}

		public static ServiceOverride OnComponent(string dependencyName, string componentName)
		{
			return Property.ForKey(dependencyName).Is(componentName);
		}

		public static ServiceOverride OnComponent(Type dependencyType, string componentName)
		{
			return Property.ForKey(dependencyType).Is(componentName);
		}

		public static ServiceOverride OnComponent(string dependencyName, Type componentType)
		{
			return Property.ForKey(dependencyName).Is(componentType);
		}

		public static ServiceOverride OnComponent(Type dependencyType, Type componentType)
		{
			return Property.ForKey(dependencyType).Is(componentType);
		}

		public static ServiceOverride OnComponent<TDependencyType, TComponentType>()
		{
			return Property.ForKey<TDependencyType>().Is<TComponentType>();
		}

		public static ServiceOverride OnComponentCollection(string collectionDependencyName, params string[] componentNames)
		{
			return ServiceOverride.ForKey(collectionDependencyName).Eq(componentNames);
		}

		public static ServiceOverride OnComponentCollection(Type collectionDependencyType, params string[] componentNames)
		{
			return ServiceOverride.ForKey(collectionDependencyType).Eq(componentNames);
		}

		public static ServiceOverride OnComponentCollection<TCollectionDependencyType>(params string[] componentNames)
			where TCollectionDependencyType : IEnumerable
		{
			return ServiceOverride.ForKey(typeof(TCollectionDependencyType)).Eq(componentNames);
		}

		public static ServiceOverride OnComponentCollection(string collectionDependencyName, params Type[] componentTypes)
		{
			return ServiceOverride.ForKey(collectionDependencyName).Eq(componentTypes);
		}

		public static ServiceOverride OnComponentCollection(Type collectionDependencyType, params Type[] componentTypes)
		{
			return ServiceOverride.ForKey(collectionDependencyType).Eq(componentTypes);
		}

		public static ServiceOverride OnComponentCollection<TCollectionDependencyType>(params Type[] componentTypes)
			where TCollectionDependencyType : IEnumerable
		{
			return ServiceOverride.ForKey(typeof(TCollectionDependencyType)).Eq(componentTypes);
		}

		public static Property OnValue(string dependencyName, object value)
		{
			return Property.ForKey(dependencyName).Eq(value);
		}

		public static Property OnValue(Type dependencyType, object value)
		{
			return Property.ForKey(dependencyType).Eq(value);
		}

		public static Property OnValue<TDependencyType>(object value)
		{
			return Property.ForKey<TDependencyType>().Eq(value);
		}

		internal bool Accept<TItem>(ICollection<TItem> items) where TItem : class
		{
			var castItem = item as TItem;
			if (castItem != null)
			{
				items.Add(castItem);
				return true;
			}
			return false;
		}
	}
}