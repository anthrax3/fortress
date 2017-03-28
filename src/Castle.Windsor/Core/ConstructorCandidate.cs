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

namespace Castle.Core
{
	public class ConstructorCandidate : IComparable<ConstructorCandidate>
	{
		public ConstructorCandidate(ConstructorInfo constructorInfo, ConstructorDependencyModel[] dependencies)
		{
			Constructor = constructorInfo;
			Dependencies = dependencies;
            foreach(var d in dependencies)
                InitParameter(d);
		}

		public ConstructorInfo Constructor { get; }

		public ConstructorDependencyModel[] Dependencies { get; }

		int IComparable<ConstructorCandidate>.CompareTo(ConstructorCandidate other)
		{
			// we sort greedier first
			var value = other.Dependencies.Length - Dependencies.Length;
			if (value != 0)
				return value;
			for (var index = 0; index < Dependencies.Length; index++)
			{
				var mine = Dependencies[index];
				var othr = other.Dependencies[index];
				value = string.Compare(mine.TargetItemType.FullName, othr.TargetItemType.FullName, StringComparison.OrdinalIgnoreCase);
				if (value != 0)
					return value;
			}
			return 0;
		}

		private void InitParameter(ConstructorDependencyModel parameter)
		{
			parameter.SetParentConstructor(this);
		}
	}
}