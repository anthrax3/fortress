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
using System.Collections.Generic;

namespace Castle.Compatibility
{
	public static class EnumerableExtensions
	{
		public static Y[] ConvertAll<X, Y>(this X[] items, Func<X, Y> converter)
		{
			var results = new List<Y>();

			foreach (var item in items)
			{
				var y = converter(item);

				results.Add(y);
			}

			return results.ToArray();
		}

		public static void ForEach<X>(this IEnumerable<X> items, Action<X> action)
		{
			foreach (var item in items)
				action(item);
		}
	}
}