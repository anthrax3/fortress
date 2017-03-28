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
using Castle.Core.Internal;
using Xunit;


namespace Castle.Core.Tests.Core.Tests.Internal
{
	public class WeakKeyDictionaryTestCase : IDisposable
	{
		public void Dispose()
		{
			Dictionary = null;
			ResetItem();
		}

		private WeakKeyDictionary<TKey, TValue> Dictionary;
		private KeyValuePair<TKey, TValue> Item;

		private static readonly KeyValuePair<TKey, TValue>
			OtherItem = new KeyValuePair<TKey, TValue>(new TKey(), new TValue());

		private void CreateDictionary()
		{
			Dictionary = new WeakKeyDictionary<TKey, TValue>();
		}

		private void AddItem()
		{
			Item = new KeyValuePair<TKey, TValue>(new TKey(), new TValue());
			Dictionary.Add(Item.Key, Item.Value);
		}

		private void RemoveItem()
		{
			Dictionary.Remove(Item.Key);
		}

		private void ResetItem()
		{
			Item = default(KeyValuePair<TKey, TValue>);
		}

		private void TriggerAutomaticTrim()
		{
#pragma warning disable 219
			int dummy;
#pragma warning restore 219
			for (var i = 0; i < 128; i++)
				dummy = Dictionary.Count;
		}

		private ICollection<KeyValuePair<TKey, TValue>> Collection
		{
			get { return Dictionary; }
		}

		private static IEnumerable AsEnumerable<T>(ICollection<T> collection)
		{
			return collection;
		}

		private sealed class TKey
		{
		}

		private sealed class TValue
		{
		}

		[Fact]
		public void AfterAdd_Contains()
		{
			CreateDictionary();
			AddItem();

			Assert.True(Collection.Contains(Item));
		}

		[Fact]
		public void AfterAdd_ContainsKey()
		{
			CreateDictionary();
			AddItem();

			Assert.True(Dictionary.ContainsKey(Item.Key));
		}

		[Fact]
		public void AfterAdd_CopyTo()
		{
			CreateDictionary();
			AddItem();
			var original = new[] {OtherItem, OtherItem, OtherItem};
			var modified = new[] {OtherItem, OtherItem, OtherItem};

			Dictionary.CopyTo(modified, 1);

			Assert.Equal(original[0], modified[0]);
			Assert.Equal(Item, modified[1]);
			Assert.Equal(original[2], modified[2]);
		}

		[Fact]
		public void AfterAdd_Count()
		{
			CreateDictionary();
			AddItem();

			Assert.Equal(1, Dictionary.Count);
		}

		[Fact]
		public void AfterAdd_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.Equal(Item, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterAdd_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.Equal(Item, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterAdd_Indexer()
		{
			CreateDictionary();
			AddItem();

			Assert.Equal(Item.Value, Dictionary[Item.Key]);
		}

		[Fact]
		public void AfterAdd_Keys_Contains()
		{
			CreateDictionary();
			AddItem();

			Assert.True(Dictionary.Keys.Contains(Item.Key));
		}

		[Fact]
		public void AfterAdd_Keys_CopyTo()
		{
			CreateDictionary();
			AddItem();
			var original = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};
			var modified = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.Equal(original[0], modified[0]);
			Assert.Equal(Item.Key, modified[1]);
			Assert.Equal(original[2], modified[2]);
		}

		[Fact]
		public void AfterAdd_Keys_Count()
		{
			CreateDictionary();
			AddItem();

			Assert.Equal(1, Dictionary.Keys.Count);
		}

		[Fact]
		public void AfterAdd_Keys_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.Equal(Item.Key, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterAdd_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.Equal(Item.Key, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterAdd_TryGetValue()
		{
			CreateDictionary();
			AddItem();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.True(result);
			Assert.Equal(Item.Value, value);
		}

		[Fact]
		public void AfterAdd_Values_Contains()
		{
			CreateDictionary();
			AddItem();

			Assert.True(Dictionary.Values.Contains(Item.Value));
		}

		[Fact]
		public void AfterAdd_Values_CopyTo()
		{
			CreateDictionary();
			AddItem();
			var original = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};
			var modified = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};

			Dictionary.Values.CopyTo(modified, 1);

			Assert.Equal(original[0], modified[0]);
			Assert.Equal(Item.Value, modified[1]);
			Assert.Equal(original[2], modified[2]);
		}

		[Fact]
		public void AfterAdd_Values_Count()
		{
			CreateDictionary();
			AddItem();

			Assert.Equal(1, Dictionary.Values.Count);
		}

		[Fact]
		public void AfterAdd_Values_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.Equal(Item.Value, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterAdd_Values_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.Equal(Item.Value, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterAutomaticTrim_DeadObject()
		{
			CreateDictionary();
			AddItem();
			ResetItem();

			GC.Collect();
			TriggerAutomaticTrim();

			Assert.Equal(0, Dictionary.Count);
		}

		[Fact]
		public void AfterAutomaticTrim_LiveObject()
		{
			CreateDictionary();
			AddItem();

			GC.Collect();
			TriggerAutomaticTrim();

			Assert.Equal(1, Dictionary.Count);
			Assert.Same(Item.Value, Dictionary[Item.Key]);
		}

		[Fact]
		public void AfterCollect_Contains()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			Assert.True(Collection.Contains(Item));
		}

		[Fact]
		public void AfterCollect_ContainsKey()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			Assert.True(Dictionary.ContainsKey(Item.Key));
		}

		[Fact]
		public void AfterCollect_CopyTo()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();
			var original = new[] {OtherItem, OtherItem, OtherItem};
			var modified = new[] {OtherItem, OtherItem, OtherItem};

			Dictionary.CopyTo(modified, 1);

			Assert.Equal(original[0], modified[0]);
			Assert.Equal(Item, modified[1]);
			Assert.Equal(original[2], modified[2]);
		}

		[Fact]
		public void AfterCollect_Count()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.Equal(2, Dictionary.Count);
		}

		[Fact]
		public void AfterCollect_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.Equal(Item, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterCollect_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.Equal(Item, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterCollect_Indexer()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			Assert.Equal(Item.Value, Dictionary[Item.Key]);
		}

		[Fact]
		public void AfterCollect_Keys_Contains()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			Assert.True(Dictionary.Keys.Contains(Item.Key));
		}

		[Fact]
		public void AfterCollect_Keys_CopyTo()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();
			var original = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};
			var modified = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.Equal(original[0], modified[0]);
			Assert.Equal(Item.Key, modified[1]);
			Assert.Equal(original[2], modified[2]);
		}

		[Fact]
		public void AfterCollect_Keys_Count()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.Equal(2, Dictionary.Keys.Count);
		}

		[Fact]
		public void AfterCollect_Keys_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.Equal(Item.Key, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterCollect_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.Equal(Item.Key, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterCollect_TryGetValue()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.True(result);
			Assert.Equal(Item.Value, value);
		}

		[Fact]
		public void AfterCollect_Values_Contains()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			Assert.True(Dictionary.Values.Contains(Item.Value));
		}

		[Fact]
		public void AfterCollect_Values_CopyTo()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();
			var original = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};
			var modified = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};

			Dictionary.Values.CopyTo(modified, 1);

			// Values for collected keys are present until TrimDeadObjects() is called
			Assert.Equal(original[0], modified[0]);
			Assert.NotEqual(original[1], modified[1]);
			Assert.NotEqual(Item.Value, modified[1]);
			Assert.Equal(Item.Value, modified[2]);
		}

		[Fact]
		public void AfterCollect_Values_Count()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			// Collected items are counted until TrimDeadObjects() is called
			Assert.Equal(2, Dictionary.Values.Count);
		}

		[Fact]
		public void AfterCollect_Values_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				// Values for collected keys are present until TrimDeadObjects() is called
				Assert.NotNull(e);
				Assert.True(e.MoveNext());
				Assert.NotEqual(Item.Value, e.Current);
				Assert.NotEqual(OtherItem.Value, e.Current);
				Assert.True(e.MoveNext());
				Assert.Equal(Item.Value, e.Current);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterCollect_Values_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			ResetItem();
			AddItem();
			GC.Collect();

			// Values for collected keys are present until TrimDeadObjects() is called
			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.NotNull(e);
			Assert.True(e.MoveNext());
			Assert.NotEqual(Item.Value, e.Current);
			Assert.NotEqual(OtherItem.Value, e.Current);
			Assert.True(e.MoveNext());
			Assert.Equal(Item.Value, e.Current);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterExplicitTrim_LiveObject()
		{
			CreateDictionary();
			AddItem();

			GC.Collect();
			Dictionary.TrimDeadObjects();

			Assert.Equal(1, Dictionary.Count);
			Assert.Same(Item.Value, Dictionary[Item.Key]);
		}

		[Fact]
		public void AfterRemove_Contains()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.False(Collection.Contains(Item));
		}

		[Fact]
		public void AfterRemove_ContainsKey()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.False(Dictionary.ContainsKey(Item.Key));
		}

		[Fact]
		public void AfterRemove_CopyTo()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();
			var original = new[] {OtherItem, OtherItem, OtherItem};
			var modified = new[] {OtherItem, OtherItem, OtherItem};

			Dictionary.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void AfterRemove_Count()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.Equal(0, Dictionary.Count);
		}

		[Fact]
		public void AfterRemove_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterRemove_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterRemove_Indexer()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.Throws<KeyNotFoundException>(() =>
			{
				var dummy = Dictionary[Item.Key];
			});
		}

		[Fact]
		public void AfterRemove_Keys_Contains()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.False(Dictionary.Keys.Contains(Item.Key));
		}

		[Fact]
		public void AfterRemove_Keys_CopyTo()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();
			var original = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};
			var modified = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void AfterRemove_Keys_Count()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.Equal(0, Dictionary.Keys.Count);
		}

		[Fact]
		public void AfterRemove_Keys_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterRemove_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void AfterRemove_TryGetValue()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			TValue value;
			var result = Dictionary.TryGetValue(Item.Key, out value);

			Assert.False(result);
			Assert.Equal(default(TValue), value);
		}

		[Fact]
		public void AfterRemove_Values_Contains()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.False(Dictionary.Values.Contains(Item.Value));
		}

		[Fact]
		public void AfterRemove_Values_CopyTo()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();
			var original = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};
			var modified = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};

			Dictionary.Values.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void AfterRemove_Values_Count()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			Assert.Equal(0, Dictionary.Values.Count);
		}

		[Fact]
		public void AfterRemove_Values_Enumerator_Generic()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void AfterRemove_Values_Enumerator_Nongeneric()
		{
			CreateDictionary();
			AddItem();
			RemoveItem();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void Initially_Contains()
		{
			CreateDictionary();

			Assert.False(Collection.Contains(OtherItem));
		}

		[Fact]
		public void Initially_ContainsKey()
		{
			CreateDictionary();

			Assert.False(Dictionary.ContainsKey(OtherItem.Key));
		}

		[Fact]
		public void Initially_CopyTo()
		{
			CreateDictionary();
			var original = new[] {OtherItem, OtherItem, OtherItem};
			var modified = new[] {OtherItem, OtherItem, OtherItem};

			Dictionary.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void Initially_Count()
		{
			CreateDictionary();

			Assert.Equal(0, Dictionary.Count);
		}

		[Fact]
		public void Initially_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void Initially_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void Initially_Indexer()
		{
			CreateDictionary();

			Assert.Throws<KeyNotFoundException>(() =>
			{
				var dummy = Dictionary[OtherItem.Key];
			});
		}

		[Fact]
		public void Initially_Keys_Contains()
		{
			CreateDictionary();

			Assert.False(Dictionary.Keys.Contains(OtherItem.Key));
		}

		[Fact]
		public void Initially_Keys_CopyTo()
		{
			CreateDictionary();
			var original = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};
			var modified = new[] {OtherItem.Key, OtherItem.Key, OtherItem.Key};

			Dictionary.Keys.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void Initially_Keys_Count()
		{
			CreateDictionary();

			Assert.Equal(0, Dictionary.Keys.Count);
		}

		[Fact]
		public void Initially_Keys_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.Keys.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void Initially_Keys_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary.Keys).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void Initially_TryGetValue()
		{
			CreateDictionary();

			TValue value;
			var result = Dictionary.TryGetValue(OtherItem.Key, out value);

			Assert.False(result);
			Assert.Equal(default(TValue), value);
		}

		[Fact]
		public void Initially_Values_Contains()
		{
			CreateDictionary();

			Assert.False(Dictionary.Values.Contains(OtherItem.Value));
		}

		[Fact]
		public void Initially_Values_CopyTo()
		{
			CreateDictionary();
			var original = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};
			var modified = new[] {OtherItem.Value, OtherItem.Value, OtherItem.Value};

			Dictionary.Values.CopyTo(modified, 1);

			Assert.Equal(original, modified);
		}

		[Fact]
		public void Initially_Values_Count()
		{
			CreateDictionary();

			Assert.Equal(0, Dictionary.Values.Count);
		}

		[Fact]
		public void Initially_Values_Enumerator_Generic()
		{
			CreateDictionary();

			using (var e = Dictionary.Values.GetEnumerator())
			{
				Assert.NotNull(e);
				Assert.False(e.MoveNext());
			}
		}

		[Fact]
		public void Initially_Values_Enumerator_Nongeneric()
		{
			CreateDictionary();

			var e = AsEnumerable(Dictionary.Values).GetEnumerator();
			Assert.NotNull(e);
			Assert.False(e.MoveNext());
		}

		[Fact]
		public void IsReadOnly()
		{
			CreateDictionary();

			Assert.False(Collection.IsReadOnly);
		}

		[Fact]
		public void Keys_Add()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Add(OtherItem.Key));
		}

		[Fact]
		public void Keys_Clear()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Clear());
		}

		[Fact]
		public void Keys_IsReadOnly()
		{
			CreateDictionary();

			Assert.True(Dictionary.Keys.IsReadOnly);
		}

		[Fact]
		public void Keys_Remove()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Keys.Remove(OtherItem.Key));
		}

		[Fact]
		public void Values_Add()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Add(OtherItem.Value));
		}

		[Fact]
		public void Values_Clear()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Clear());
		}

		[Fact]
		public void Values_IsReadOnly()
		{
			CreateDictionary();

			Assert.True(Dictionary.Values.IsReadOnly);
		}

		[Fact]
		public void Values_Remove()
		{
			CreateDictionary();

			Assert.Throws<NotSupportedException>(() => Dictionary.Values.Remove(OtherItem.Value));
		}
	}
}