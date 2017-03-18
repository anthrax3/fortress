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
using System.Collections;
using System.ComponentModel;
using Castle.Core.Core;

namespace Castle.Windsor.MicroKernel.Registration
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ComponentDependencyRegistrationExtensions
	{
		public static IDictionary Insert(this IDictionary arguments, string key, object value)
		{
			arguments[key] = value;
			return arguments;
		}

		public static IDictionary Insert(this IDictionary arguments, Type dependencyType, object value)
		{
			arguments[dependencyType] = value;
			return arguments;
		}

		public static IDictionary InsertAnonymous(this IDictionary arguments, object namedArgumentsAsAnonymousType)
		{
			foreach (DictionaryEntry item in new ReflectionBasedDictionaryAdapter(namedArgumentsAsAnonymousType))
			{
				arguments[item.Key] = item.Value;
			}

			return arguments;
		}

		public static IDictionary InsertTyped<TDependencyType>(this IDictionary arguments, TDependencyType value)
		{
			arguments[typeof(TDependencyType)] = value;
			return arguments;
		}

		public static IDictionary InsertTypedCollection(this IDictionary arguments, object[] typedArgumentsArray)
		{
			foreach (var item in typedArgumentsArray)
			{
				arguments[item.GetType()] = item;
			}

			return arguments;
		}
	}
}
