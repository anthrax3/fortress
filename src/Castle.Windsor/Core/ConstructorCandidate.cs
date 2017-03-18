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

namespace Castle.Core
{
	using System;
	using System.Reflection;

	[Serializable]
	public class ConstructorCandidate : IComparable<ConstructorCandidate>
	{
		private readonly ConstructorInfo constructorInfo;
		private readonly ConstructorDependencyModel[] dependencies;

		public ConstructorCandidate(ConstructorInfo constructorInfo, ConstructorDependencyModel[] dependencies)
		{
			this.constructorInfo = constructorInfo;
			this.dependencies = dependencies;
			Array.ForEach(dependencies, InitParameter);
		}

		public ConstructorInfo Constructor
		{
			get { return constructorInfo; }
		}

		public ConstructorDependencyModel[] Dependencies
		{
			get { return dependencies; }
		}

		private void InitParameter(ConstructorDependencyModel parameter)
		{
			parameter.SetParentConstructor(this);
		}

		int IComparable<ConstructorCandidate>.CompareTo(ConstructorCandidate other)
		{
			// we sort greedier first
			var value = other.Dependencies.Length - Dependencies.Length;
			if (value != 0)
			{
				return value;
			}
			for (var index = 0; index < Dependencies.Length; index++)
			{
				var mine = Dependencies[index];
				var othr = other.Dependencies[index];
				value = string.Compare(mine.TargetItemType.FullName, othr.TargetItemType.FullName, StringComparison.OrdinalIgnoreCase);
				if (value != 0)
				{
					return value;
				}
			}
			return 0;
		}
	}
}
