// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Core
{
	public class Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>
	{
		public Pair(TFirst first, TSecond second)
		{
			First = first;
			Second = second;
		}

		public TFirst First { get; }

		public TSecond Second { get; }

		public bool Equals(Pair<TFirst, TSecond> other)
		{
			if (other == null)
				return false;
			return Equals(First, other.First) && Equals(Second, other.Second);
		}

		public override string ToString()
		{
			return First + " " + Second;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;
			return Equals(obj as Pair<TFirst, TSecond>);
		}

		public override int GetHashCode()
		{
			return First.GetHashCode() + 29 * Second.GetHashCode();
		}
	}
}