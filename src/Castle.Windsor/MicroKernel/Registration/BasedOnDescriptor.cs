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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.Lifestyle.Scoped;

namespace Castle.Windsor.MicroKernel.Registration
{
	public class BasedOnDescriptor : IRegistration
	{
		private readonly List<Type> potentialBases;
		private Action<ComponentRegistration> configuration;
		private readonly FromDescriptor from;
		private readonly ServiceDescriptor service;
		private Predicate<Type> ifFilter;
		private Predicate<Type> unlessFilter;

		internal BasedOnDescriptor(IEnumerable<Type> basedOn, FromDescriptor from, Predicate<Type> additionalFilters)
		{
			potentialBases = basedOn.ToList();
			this.from = from;
			service = new ServiceDescriptor(this);
			If(additionalFilters);
		}

		public ServiceDescriptor WithService
		{
			get { return service; }
		}

		public FromDescriptor AllowMultipleMatches()
		{
			return from.AllowMultipleMatches();
		}

		[Obsolete("Calling this method resets registration. If that's what you want, start anew, with Classes.FromAssembly..")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor BasedOn<T>()
		{
			return from.BasedOn<T>();
		}

		[Obsolete("Calling this method resets registration. If that's what you want, start anew, with Classes.FromAssembly...")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor BasedOn(Type basedOn)
		{
			return from.BasedOn(basedOn);
		}

		public BasedOnDescriptor OrBasedOn(Type basedOn)
		{
			potentialBases.Add(basedOn);
			return this;
		}

		public BasedOnDescriptor Configure(Action<ComponentRegistration> configurer)
		{
			configuration += configurer;
			return this;
		}

		public BasedOnDescriptor ConfigureFor<TComponentImplementationType>(Action<ComponentRegistration> configurer)
		{
			return ConfigureIf(r => typeof(TComponentImplementationType).IsAssignableFrom(r.Implementation), configurer);
		}

		public BasedOnDescriptor ConfigureIf(Predicate<ComponentRegistration> condition,
		                                     Action<ComponentRegistration> configurer)
		{
			configuration += r =>
			{
				if (condition(r))
				{
					configurer(r);
				}
			};
			return this;
		}

		public BasedOnDescriptor ConfigureIf(Predicate<ComponentRegistration> condition,
		                                     Action<ComponentRegistration> configurerWhenTrue,
		                                     Action<ComponentRegistration> configurerWhenFalse)
		{
			configuration += r =>
			{
				if (condition(r))
				{
					configurerWhenTrue(r);
				}
				else
				{
					configurerWhenFalse(r);
				}
			};
			return this;
		}

		public BasedOnDescriptor If(Predicate<Type> ifFilter)
		{
			this.ifFilter += ifFilter;
			return this;
		}

		public BasedOnDescriptor Unless(Predicate<Type> unlessFilter)
		{
			this.unlessFilter += unlessFilter;
			return this;
		}

		[Obsolete("Calling this method resets registration. If that's what you want, start anew, with Classes.FromAssembly..."
			)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BasedOnDescriptor Where(Predicate<Type> accepted)
		{
			return from.Where(accepted);
		}

		public BasedOnDescriptor WithServiceAllInterfaces()
		{
			return WithService.AllInterfaces();
		}

		public BasedOnDescriptor WithServiceBase()
		{
			return WithService.Base();
		}

		public BasedOnDescriptor WithServiceDefaultInterfaces()
		{
			return WithService.DefaultInterfaces();
		}

		public BasedOnDescriptor WithServiceFirstInterface()
		{
			return WithService.FirstInterface();
		}

		public BasedOnDescriptor WithServiceFromInterface(Type implements)
		{
			return WithService.FromInterface(implements);
		}

		public BasedOnDescriptor WithServiceFromInterface()
		{
			return WithService.FromInterface();
		}

		public BasedOnDescriptor WithServiceSelect(ServiceDescriptor.ServiceSelector selector)
		{
			return WithService.Select(selector);
		}

		public BasedOnDescriptor WithServiceSelf()
		{
			return WithService.Self();
		}

		public BasedOnDescriptor LifestyleCustom(Type customLifestyleType)
		{
			return Configure(c => c.LifestyleCustom(customLifestyleType));
		}

		public BasedOnDescriptor LifestyleCustom<TLifestyleManager>() where TLifestyleManager : ILifestyleManager, new()
		{
			return Configure(c => c.LifestyleCustom<TLifestyleManager>());
		}

		public BasedOnDescriptor LifestylePerThread()
		{
			return Configure(c => c.LifestylePerThread());
		}

		public BasedOnDescriptor LifestyleScoped()
		{
			return Configure(c => c.LifestyleScoped());
		}

		public BasedOnDescriptor LifestyleScoped(Type scopeAccessorType)
		{
			return Configure(c => c.LifestyleScoped(scopeAccessorType));
		}

		public BasedOnDescriptor LifestyleScoped<TScopeAccessor>() where TScopeAccessor : IScopeAccessor, new()
		{
			return Configure(c => c.LifestyleScoped<TScopeAccessor>());
		}

		public BasedOnDescriptor LifestyleBoundTo<TBaseForRoot>() where TBaseForRoot : class
		{
			return Configure(c => c.LifestyleBoundTo<TBaseForRoot>());
		}

		public BasedOnDescriptor LifestyleBoundToNearest<TBaseForRoot>() where TBaseForRoot : class
		{
			return Configure(c => c.LifestyleBoundToNearest<TBaseForRoot>());
		}

		public BasedOnDescriptor LifestylePerWebRequest()
		{
			return Configure(c => c.LifestylePerWebRequest());
		}

		public BasedOnDescriptor LifestylePooled(int? initialSize = null, int? maxSize = null)
		{
			return Configure(c => c.LifestylePooled(initialSize, maxSize));
		}

		public BasedOnDescriptor LifestyleSingleton()
		{
			return Configure(c => c.LifestyleSingleton());
		}

		public BasedOnDescriptor LifestyleTransient()
		{
			return Configure(c => c.LifestyleTransient());
		}

		public BasedOnDescriptor WithServices(IEnumerable<Type> types)
		{
			return WithService.Select(types);
		}

		public BasedOnDescriptor WithServices(params Type[] types)
		{
			return WithService.Select(types);
		}

		protected virtual bool Accepts(Type type, out Type[] baseTypes)
		{
			return IsBasedOn(type, out baseTypes)
			       && ExecuteIfCondition(type)
			       && ExecuteUnlessCondition(type) == false;
		}

		protected bool ExecuteIfCondition(Type type)
		{
			if (ifFilter == null)
			{
				return true;
			}

			foreach (Predicate<Type> filter in ifFilter.GetInvocationList())
			{
				if (filter(type) == false)
				{
					return false;
				}
			}

			return true;
		}

		protected bool ExecuteUnlessCondition(Type type)
		{
			if (unlessFilter == null)
			{
				return false;
			}
			foreach (Predicate<Type> filter in unlessFilter.GetInvocationList())
			{
				if (filter(type))
				{
					return true;
				}
			}
			return false;
		}

		protected bool IsBasedOn(Type type, out Type[] baseTypes)
		{
			var actuallyBasedOn = new List<Type>();
			foreach (var potentialBase in potentialBases)
			{
				if (potentialBase.IsAssignableFrom(type))
				{
					actuallyBasedOn.Add(potentialBase);
				}
				else if (potentialBase.IsGenericTypeDefinition)
				{
					if (potentialBase.IsInterface)
					{
						if (IsBasedOnGenericInterface(type, potentialBase, out baseTypes))
						{
							actuallyBasedOn.AddRange(baseTypes);
						}
					}

					if (IsBasedOnGenericClass(type, potentialBase, out baseTypes))
					{
						actuallyBasedOn.AddRange(baseTypes);
					}
				}
			}
			baseTypes = actuallyBasedOn.Distinct().ToArray();
			return baseTypes.Length > 0;
		}

		internal bool TryRegister(Type type, IKernel kernel)
		{
			Type[] baseTypes;

			if (!Accepts(type, out baseTypes))
			{
				return false;
			}
			var defaults = CastleComponentAttribute.GetDefaultsFor(type);
			var serviceTypes = service.GetServices(type, baseTypes);
			if (serviceTypes.Count == 0 && defaults.Services.Length > 0)
			{
				serviceTypes = defaults.Services;
			}
			var registration = Component.For(serviceTypes);
			registration.ImplementedBy(type);

			if (configuration != null)
			{
				configuration(registration);
			}
			if (String.IsNullOrEmpty(registration.Name) && !String.IsNullOrEmpty(defaults.Name))
			{
				registration.Named(defaults.Name);
			}
			else
			{
				registration.RegisterOptionally();
			}
			kernel.Register(registration);
			return true;
		}

		private static bool IsBasedOnGenericClass(Type type, Type basedOn, out Type[] baseTypes)
		{
			while (type != null)
			{
				if (type.IsGenericType &&
				    type.GetGenericTypeDefinition() == basedOn)
				{
					baseTypes = new[] { type };
					return true;
				}

				type = type.BaseType;
			}
			baseTypes = null;
			return false;
		}

		private static bool IsBasedOnGenericInterface(Type type, Type basedOn, out Type[] baseTypes)
		{
			var types = new List<Type>(4);
			foreach (var @interface in type.GetInterfaces())
			{
				if (@interface.IsGenericType &&
				    @interface.GetGenericTypeDefinition() == basedOn)
				{
					if (@interface.ReflectedType == null &&
					    @interface.ContainsGenericParameters)
					{
						types.Add(@interface.GetGenericTypeDefinition());
					}
					else
					{
						types.Add(@interface);
					}
				}
			}
			baseTypes = types.ToArray();
			return baseTypes.Length > 0;
		}

		void IRegistration.Register(IKernelInternal kernel)
		{
			((IRegistration)from).Register(kernel);
		}
	}
}
