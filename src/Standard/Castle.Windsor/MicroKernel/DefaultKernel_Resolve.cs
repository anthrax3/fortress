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
using Castle.Core;
using Castle.MicroKernel.Handlers;

namespace Castle.MicroKernel
{
	public partial class DefaultKernel
	{
		public virtual object Resolve(string key, Type service)
		{
			return (this as IKernelInternal).Resolve(key, service, null, ReleasePolicy);
		}

		public virtual object Resolve(string key, Type service, IDictionary arguments)
		{
			return (this as IKernelInternal).Resolve(key, service, arguments, ReleasePolicy);
		}

		public T Resolve<T>(IDictionary arguments)
		{
			return (T) Resolve(typeof(T), arguments);
		}

		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return (T) Resolve(typeof(T), new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public T Resolve<T>()
		{
			return (T) Resolve(typeof(T), null);
		}

		public T Resolve<T>(string key)
		{
			return (T) (this as IKernelInternal).Resolve(key, typeof(T), null, ReleasePolicy);
		}

		public T Resolve<T>(string key, IDictionary arguments)
		{
			return (T) (this as IKernelInternal).Resolve(key, typeof(T), arguments, ReleasePolicy);
		}

		public object Resolve(Type service)
		{
			return (this as IKernelInternal).Resolve(service, null, ReleasePolicy);
		}

		public object Resolve(Type service, IDictionary arguments)
		{
			return (this as IKernelInternal).Resolve(service, arguments, ReleasePolicy);
		}

		public object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		public Array ResolveAll(Type service)
		{
			return (this as IKernelInternal).ResolveAll(service, null, ReleasePolicy);
		}

		public Array ResolveAll(Type service, IDictionary arguments)
		{
			return (this as IKernelInternal).ResolveAll(service, arguments, ReleasePolicy);
		}

		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return (this as IKernelInternal).ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType), ReleasePolicy);
		}

		public TService[] ResolveAll<TService>(object argumentsAsAnonymousType)
		{
			return
				(TService[]) (this as IKernelInternal).ResolveAll(typeof(TService), new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType), ReleasePolicy);
		}

		public TService[] ResolveAll<TService>(IDictionary arguments)
		{
			return (TService[]) (this as IKernelInternal).ResolveAll(typeof(TService), arguments, ReleasePolicy);
		}

		public TService[] ResolveAll<TService>()
		{
			return (TService[]) (this as IKernelInternal).ResolveAll(typeof(TService), null, ReleasePolicy);
		}

		object IKernelInternal.Resolve(string key, Type service, IDictionary arguments, IReleasePolicy policy)
		{
			var handler = (this as IKernelInternal).LoadHandlerByName(key, service, arguments);
			if (handler == null)
			{
				var otherHandlers = GetHandlers(service).Length;
				throw new ComponentNotFoundException(key, service, otherHandlers);
			}
			return ResolveComponent(handler, service ?? typeof(object), arguments, policy);
		}

		object IKernelInternal.Resolve(Type service, IDictionary arguments, IReleasePolicy policy)
		{
			var handler = (this as IKernelInternal).LoadHandlerByType(null, service, arguments);
			if (handler == null)
				throw new ComponentNotFoundException(service);
			return ResolveComponent(handler, service, arguments, policy);
		}

		Array IKernelInternal.ResolveAll(Type service, IDictionary arguments, IReleasePolicy policy)
		{
			var resolved = new List<object>();
			foreach (var handler in GetHandlers(service))
			{
				if (handler.IsBeingResolvedInContext(currentCreationContext))
					continue;

				try
				{
					var component = ResolveComponent(handler, service, arguments, policy);
					resolved.Add(component);
				}
				catch (GenericHandlerTypeMismatchException)
				{
					// that's the only case where we ignore the component and allow it to not be resolved.
					// only because we have no way to actually test if generic constraints can be satisfied.
				}
			}

			if (resolved.Count == 0)
				EmptyCollectionResolving(service);
			var components = Array.CreateInstance(service, resolved.Count);
			((ICollection) resolved).CopyTo(components, 0);
			return components;
		}
	}
}