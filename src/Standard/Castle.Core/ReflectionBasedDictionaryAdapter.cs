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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Castle.Core.Core
{
	public sealed class ReflectionBasedDictionaryAdapter : IDictionary
	{
		private readonly Dictionary<string, object> properties =
			new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		public ReflectionBasedDictionaryAdapter(object target)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			Read(properties, target);
		}

		public int Count
		{
			get { return properties.Count; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return properties; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public object this[object key]
		{
			get
			{
				object value;
				properties.TryGetValue(key.ToString(), out value);
				return value;
			}
			set { throw new NotImplementedException(); }
		}

		public ICollection Keys
		{
			get { return properties.Keys; }
		}

		public ICollection Values
		{
			get { return properties.Values; }
		}

		bool IDictionary.IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(object key)
		{
			return properties.ContainsKey(key.ToString());
		}

		public void Remove(object key)
		{
		}

		public IEnumerator GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(properties.GetEnumerator());
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(properties.GetEnumerator());
		}

		public static void Read(IDictionary targetDictionary, object valuesAsAnonymousObject)
		{
			var targetType = valuesAsAnonymousObject.GetType();
			foreach (var property in GetReadableProperties(targetType))
			{
				var value = GetPropertyValue(valuesAsAnonymousObject, property);
				targetDictionary[property.Name] = value;
			}
		}

		private static object GetPropertyValue(object target, PropertyInfo property)
		{
			return property.GetValue(target, null);
		}

		private static IEnumerable<PropertyInfo> GetReadableProperties(Type targetType)
		{
			return targetType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(IsReadable);
		}

		private static bool IsReadable(PropertyInfo property)
		{
			return property.CanRead && property.GetIndexParameters().Length == 0;
		}

		private class DictionaryEntryEnumeratorAdapter : IDictionaryEnumerator
		{
			private readonly IDictionaryEnumerator enumerator;
			private KeyValuePair<string, object> current;

			public DictionaryEntryEnumeratorAdapter(IDictionaryEnumerator enumerator)
			{
				this.enumerator = enumerator;
			}

			public DictionaryEntry Entry
			{
				get { return new DictionaryEntry(Key, Value); }
			}

			public object Key
			{
				get { return current.Key; }
			}

			public object Value
			{
				get { return current.Value; }
			}

			public object Current
			{
				get { return new DictionaryEntry(Key, Value); }
			}

			public bool MoveNext()
			{
				var moved = enumerator.MoveNext();

				if (moved)
					current = (KeyValuePair<string, object>) enumerator.Current;

				return moved;
			}

			public void Reset()
			{
				enumerator.Reset();
			}
		}
	}
}