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
using Castle.Core.Internal;

namespace Castle.DynamicProxy
{
	public class ProxyGenerationOptions
	{
		public static readonly ProxyGenerationOptions Default = new ProxyGenerationOptions();
		internal readonly IList<Attribute> attributesToAddToGeneratedTypes = new List<Attribute>();

		private MixinData mixinData; // this is calculated dynamically on proxy type creation

		private List<object> mixins;

		public ProxyGenerationOptions(IProxyGenerationHook hook)
		{
			BaseTypeForInterfaceProxy = typeof(object);
			Hook = hook;
		}

		public ProxyGenerationOptions()
			: this(new AllMethodsHook())
		{
		}

		public IProxyGenerationHook Hook { get; set; }

		public IInterceptorSelector Selector { get; set; }

		public Type BaseTypeForInterfaceProxy { get; set; }

		public IList<CustomAttributeInfo> AdditionalAttributes { get; } = new List<CustomAttributeInfo>();

		public MixinData MixinData
		{
			get
			{
				if (mixinData == null)
					throw new InvalidOperationException("Call Initialize before accessing the MixinData property.");
				return mixinData;
			}
		}

		public bool HasMixins
		{
			get { return mixins != null && mixins.Count != 0; }
		}

		public void Initialize()
		{
			if (mixinData == null)
			{
				try
				{
					mixinData = new MixinData(mixins);
				}
				catch (ArgumentException ex)
				{
					throw new InvalidMixinConfigurationException("There is a problem with the mixins added to this ProxyGenerationOptions: " + ex.Message, ex);
				}
			}
		}

		public void AddMixinInstance(object instance)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (mixins == null)
				mixins = new List<object>();

			mixins.Add(instance);
			mixinData = null;
		}

		public object[] MixinsAsArray()
		{
			if (mixins == null)
				return new object[0];

			return mixins.ToArray();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			var proxyGenerationOptions = obj as ProxyGenerationOptions;
			if (ReferenceEquals(proxyGenerationOptions, null))
				return false;

			// ensure initialization before accessing MixinData
			Initialize();
			proxyGenerationOptions.Initialize();

			if (!Equals(Hook, proxyGenerationOptions.Hook))
				return false;
			if (!Equals(Selector == null, proxyGenerationOptions.Selector == null))
				return false;
			if (!Equals(MixinData, proxyGenerationOptions.MixinData))
				return false;
			if (!Equals(BaseTypeForInterfaceProxy, proxyGenerationOptions.BaseTypeForInterfaceProxy))
				return false;
			if (!CollectionExtensions.AreEquivalent(AdditionalAttributes, proxyGenerationOptions.AdditionalAttributes))
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			// ensure initialization before accessing MixinData
			Initialize();

			var result = Hook != null ? Hook.GetType().GetHashCode() : 0;
			result = 29 * result + (Selector != null ? 1 : 0);
			result = 29 * result + MixinData.GetHashCode();
			result = 29 * result + (BaseTypeForInterfaceProxy != null ? BaseTypeForInterfaceProxy.GetHashCode() : 0);
			result = 29 * result + CollectionExtensions.GetContentsHashCode(AdditionalAttributes);
			return result;
		}
	}
}