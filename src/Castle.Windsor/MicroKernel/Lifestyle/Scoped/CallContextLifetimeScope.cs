// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using Castle.Windsor.Core;
using Castle.Windsor.Windsor;

namespace Castle.Windsor.MicroKernel.Lifestyle.Scoped
{
	public class CallContextLifetimeScope : ILifetimeScope
	{
		private static readonly ConcurrentDictionary<Guid, CallContextLifetimeScope> appDomainLocalInstanceCache =
			new ConcurrentDictionary<Guid, CallContextLifetimeScope>();

		private static readonly string keyInCallContext = "castle.lifetime-scope-" + AppDomain.CurrentDomain.Id.ToString(CultureInfo.InvariantCulture);
		private readonly Guid contextId;
		private readonly Lock @lock = Lock.Create();
		private readonly CallContextLifetimeScope parentScope;
		private ScopeCache cache = new ScopeCache();

		public CallContextLifetimeScope(IKernel container) : this()
		{
		}

		public CallContextLifetimeScope()
		{
			var parent = ObtainCurrentScope();
			if (parent != null)
				parentScope = parent;
			contextId = Guid.NewGuid();
			var added = appDomainLocalInstanceCache.TryAdd(contextId, this);
			Debug.Assert(added);
			SetCurrentScope(this);
		}

		public CallContextLifetimeScope(IWindsorContainer container) : this()
		{
		}

		public void Dispose()
		{
			using (var token = @lock.ForReadingUpgradeable())
			{
				if (cache == null)
					return;
				token.Upgrade();
				cache.Dispose();
				cache = null;

				if (parentScope != null)
					SetCurrentScope(parentScope);
				else
					CallContext.FreeNamedDataSlot(keyInCallContext);
			}
			CallContextLifetimeScope @this;
			appDomainLocalInstanceCache.TryRemove(contextId, out @this);
		}

		public Burden GetCachedInstance(ComponentModel model, ScopedInstanceActivationCallback createInstance)
		{
			using (var token = @lock.ForReadingUpgradeable())
			{
				var burden = cache[model];
				if (burden == null)
				{
					token.Upgrade();

					burden = createInstance(delegate { });
					cache[model] = burden;
				}
				return burden;
			}
		}

		private void SetCurrentScope(CallContextLifetimeScope lifetimeScope)
		{
			CallContext.LogicalSetData(keyInCallContext, lifetimeScope.contextId);
		}

		public static CallContextLifetimeScope ObtainCurrentScope()
		{
			var scopeKey = CallContext.LogicalGetData(keyInCallContext);
			if (scopeKey == null)
				return null;
			CallContextLifetimeScope scope;
			appDomainLocalInstanceCache.TryGetValue((Guid) scopeKey, out scope);
			return scope;
		}
	}
}