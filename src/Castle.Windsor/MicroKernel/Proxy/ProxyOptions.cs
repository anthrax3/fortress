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
using Castle.Windsor.Core;

namespace Castle.Windsor.MicroKernel.Proxy
{
	public class ProxyOptions
	{
		private readonly ComponentModel component;
		private IReference<IProxyGenerationHook> hook;
		private List<Type> interfaceList;
		private List<IReference<object>> mixInList;

		private IReference<IInterceptorSelector> selector;

		public ProxyOptions(ComponentModel component)
		{
			this.component = component;
		}

		public Type[] AdditionalInterfaces
		{
			get
			{
				if (interfaceList != null)
					return interfaceList.ToArray();

				return Type.EmptyTypes;
			}
		}

		public bool AllowChangeTarget { get; set; }

		public IReference<IProxyGenerationHook> Hook
		{
			get { return hook; }
			set { SetReferenceValue(ref hook, value); }
		}

		public IEnumerable<IReference<object>> MixIns
		{
			get
			{
				if (mixInList != null)
					return mixInList;
				return new IReference<object>[] {};
			}
		}

		public bool OmitTarget { get; set; }

		public IReference<IInterceptorSelector> Selector
		{
			get { return selector; }
			set { SetReferenceValue(ref selector, value); }
		}

		public bool RequiresProxy
		{
			get { return interfaceList != null || mixInList != null || hook != null; }
		}

		public void AddAdditionalInterfaces(params Type[] interfaces)
		{
			if (interfaces == null || interfaces.Length == 0)
				return;

			if (interfaceList == null)
				interfaceList = new List<Type>();

			interfaceList.AddRange(interfaces);
		}

		public void AddMixIns(params object[] mixIns)
		{
			if (mixIns == null || mixIns.Length == 0)
				return;

			if (mixInList == null)
				mixInList = new List<IReference<object>>();

			foreach (var mixIn in mixIns)
			{
				var reference = new InstanceReference<object>(mixIn);
				mixInList.Add(reference);
				reference.Attach(component);
			}
		}

		public void AddMixinReference(IReference<object> mixIn)
		{
			if (mixIn == null)
				throw new ArgumentNullException("mixIn");

			if (mixInList == null)
				mixInList = new List<IReference<object>>();
			mixInList.Add(mixIn);
			mixIn.Attach(component);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			var proxyOptions = obj as ProxyOptions;
			if (proxyOptions == null)
				return false;
			if (!Equals(Hook, proxyOptions.Hook))
				return false;
			if (!Equals(Selector, proxyOptions.Selector))
				return false;
			if (!Equals(OmitTarget, proxyOptions.OmitTarget))
				return false;
			if (!AdditionalInterfacesAreEquals(proxyOptions))
				return false;
			return MixInsAreEquals(proxyOptions);
		}

		public override int GetHashCode()
		{
			return 29 * base.GetHashCode()
			       + GetCollectionHashCode(interfaceList)
			       + GetCollectionHashCode(mixInList);
		}

		private bool AdditionalInterfacesAreEquals(ProxyOptions proxyOptions)
		{
			if (!Equals(interfaceList == null, proxyOptions.interfaceList == null))
				return false;
			if (interfaceList == null)
				return true; //both are null, nothing more to check
			if (interfaceList.Count != proxyOptions.interfaceList.Count)
				return false;
			for (var i = 0; i < interfaceList.Count; ++i)
				if (!proxyOptions.interfaceList.Contains(interfaceList[0]))
					return false;
			return true;
		}

		private int GetCollectionHashCode(IEnumerable items)
		{
			var result = 0;

			if (items == null)
				return result;

			foreach (var item in items)
				result = 29 * result + item.GetHashCode();

			return result;
		}

		private bool MixInsAreEquals(ProxyOptions proxyOptions)
		{
			if (!Equals(mixInList == null, proxyOptions.mixInList == null))
				return false;
			if (mixInList == null)
				return true; //both are null, nothing more to check
			if (mixInList.Count != proxyOptions.mixInList.Count)
				return false;
			for (var i = 0; i < mixInList.Count; ++i)
				if (!proxyOptions.mixInList.Contains(mixInList[0]))
					return false;
			return true;
		}

		private void SetReferenceValue<T>(ref IReference<T> reference, IReference<T> value)
		{
			if (reference != null)
				reference.Detach(component);
			if (value != null)
				value.Attach(component);
			reference = value;
		}
	}
}