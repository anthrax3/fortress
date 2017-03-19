// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
using System.Diagnostics;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel;

namespace Castle.Windsor.Core
{
	[Serializable]
	public class InterceptorReferenceCollection : IMutableCollection<InterceptorReference>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly ComponentModel component;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] [DebuggerDisplay("Count = {list.Count}", Name = "")] private readonly List<InterceptorReference> list = new List<InterceptorReference>();

		public InterceptorReferenceCollection(ComponentModel component)
		{
			this.component = component;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool HasInterceptors
		{
			get { return list.Count != 0; }
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count
		{
			get { return list.Count; }
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void Add(InterceptorReference item)
		{
			AddLast(item);
		}

		IEnumerator<InterceptorReference> IEnumerable<InterceptorReference>.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		bool IMutableCollection<InterceptorReference>.Remove(InterceptorReference item)
		{
			return list.Remove(item);
		}

		public void AddFirst(InterceptorReference item)
		{
			Insert(0, item);
		}

		public void AddIfNotInCollection(InterceptorReference interceptorReference)
		{
			if (list.Contains(interceptorReference) == false)
				AddLast(interceptorReference);
		}

		public void AddLast(InterceptorReference item)
		{
			list.Add(item);
			Attach(item);
		}

		public void Insert(int index, InterceptorReference item)
		{
			list.Insert(index, item);
			Attach(item);
		}

		public InterceptorReference[] ToArray()
		{
			return list.ToArray();
		}

		private void Attach(IReference<IInterceptor> interceptor)
		{
			interceptor.Attach(component);
		}
	}
}