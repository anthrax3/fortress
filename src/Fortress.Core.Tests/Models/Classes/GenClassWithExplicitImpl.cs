// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Tests.GenClasses
{
	public class GenClassWithExplicitImpl : IChangeTracking
	{
		protected virtual bool IsChanged
		{
			get { return false; }
		}

		bool IChangeTracking.IsChanged
		{
			get { return IsChanged; }
		}

		void IChangeTracking.AcceptChanges()
		{
		}

		protected virtual void AcceptChanges()
		{
		}
	}

	public class GenClassWithExplicitImpl<T> : GenClassWithExplicitImpl
		where T : IComparable
	{
		protected override bool IsChanged
		{
			get { return true; }
		}

		protected override void AcceptChanges()
		{
		}
	}
}