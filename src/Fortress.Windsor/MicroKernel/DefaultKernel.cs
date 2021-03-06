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
using System.Diagnostics;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Proxy;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.MicroKernel.SubSystems.Naming;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Diagnostics;

namespace Castle.MicroKernel
{
	[DebuggerTypeProxy(typeof(KernelDebuggerProxy))]
	public partial class DefaultKernel : IKernel, IKernelEvents, IKernelInternal
	{
		[ThreadStatic] private static CreationContext currentCreationContext;

		[ThreadStatic] private static bool isCheckingLazyLoaders;

		private readonly List<IKernel> childKernels = new List<IKernel>();

		private readonly List<IFacility> facilities = new List<IFacility>();

		private readonly object lazyLoadingLock = new object();

		private readonly Dictionary<string, ISubSystem> subsystems = new Dictionary<string, ISubSystem>(StringComparer.OrdinalIgnoreCase);

		// ReSharper disable once UnassignedField.Compiler
		private ThreadSafeFlag disposed;

		private IKernel parentKernel;

		public DefaultKernel() : this(new NotSupportedProxyFactory())
		{
		}

		public DefaultKernel(IDependencyResolver resolver, IProxyFactory proxyFactory)
		{
			RegisterSubSystems();
			ReleasePolicy = new LifecycledComponentsReleasePolicy(this);
			HandlerFactory = new DefaultHandlerFactory(this);
			ComponentModelBuilder = new DefaultComponentModelBuilder(this);
			ProxyFactory = proxyFactory;
			Resolver = resolver;
			Resolver.Initialize(this, RaiseDependencyResolving);

			Logger = NullLogger.Instance;
		}

		public DefaultKernel(IProxyFactory proxyFactory)
			: this(new DefaultDependencyResolver(), proxyFactory)
		{
		}

		protected IConversionManager ConversionSubSystem { get; private set; }

		protected INamingSubSystem NamingSubSystem { get; private set; }

		public IComponentModelBuilder ComponentModelBuilder { get; set; }

		public virtual IConfigurationStore ConfigurationStore
		{
			get { return GetSubSystem(SubSystemConstants.ConfigurationStoreKey) as IConfigurationStore; }
			set { AddSubSystem(SubSystemConstants.ConfigurationStoreKey, value); }
		}

		public GraphNode[] GraphNodes
		{
			get
			{
				var nodes = new GraphNode[NamingSubSystem.ComponentCount];
				var index = 0;

				var handlers = NamingSubSystem.GetAllHandlers();
				foreach (var handler in handlers)
					nodes[index++] = handler.ComponentModel;

				return nodes;
			}
		}

		public IHandlerFactory HandlerFactory { get; }

		public virtual IKernel Parent
		{
			get { return parentKernel; }
			set
			{
				// TODO: should the raise add/removed as child kernel methods be invoked from within the subscriber/unsubscribe methods?

				if (value == null)
				{
					if (parentKernel != null)
					{
						UnsubscribeFromParentKernel();
						RaiseRemovedAsChildKernel();
					}

					parentKernel = null;
				}
				else
				{
					if (parentKernel != value && parentKernel != null)
						throw new KernelException(
							"You can not change the kernel parent once set, use the RemoveChildKernel and AddChildKernel methods together to achieve this.");
					parentKernel = value;
					SubscribeToParentKernel();
					RaiseAddedAsChildKernel();
				}
			}
		}

		public IProxyFactory ProxyFactory { get; set; }

		public IReleasePolicy ReleasePolicy { get; set; }

		public IDependencyResolver Resolver { get; }

		public virtual void Dispose()
		{
			if (!disposed.Signal())
				return;

			DisposeSubKernels();
			TerminateFacilities();
			DisposeComponentsInstancesWithinTracker();
			DisposeHandlers();
			UnsubscribeFromParentKernel();
		}

		public virtual void AddChildKernel(IKernel childKernel)
		{
			if (childKernel == null)
				throw new ArgumentNullException("childKernel");

			childKernel.Parent = this;
			childKernels.Add(childKernel);
		}

		public virtual IKernel AddFacility(IFacility facility)
		{
            if (facility == null)
                throw new ArgumentNullException("facility");
            var facilityType = facility.GetType();
            if (facilities.Any(f => f.GetType() == facilityType))
                throw new ArgumentException(
                    string.Format(
                        "Facility of type '{0}' has already been registered with the container. Only one facility of a given type can exist in the container.",
                        facilityType.FullName));

            facilities.Add(facility);

            facility.Init(this, ConfigurationStore.GetFacilityConfiguration(facility != null ? facility.GetType().FullName : null));

            return this;
        }

		public IKernel AddFacility<T>() where T : IFacility, new()
		{
			return AddFacility(new T());
		}

		public IKernel AddFacility<T>(Action<T> onCreate)
			where T : IFacility, new()
		{
			var facility = new T();
			if (onCreate != null)
				onCreate(facility);
			return AddFacility(facility);
		}

		public void AddHandlerSelector(IHandlerSelector selector)
		{
			NamingSubSystem.AddHandlerSelector(selector);
		}

		public void AddHandlersFilter(IHandlersFilter filter)
		{
			NamingSubSystem.AddHandlersFilter(filter);
		}

		public virtual void AddSubSystem(string name, ISubSystem subsystem)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (subsystem == null)
				throw new ArgumentNullException("subsystem");

			subsystem.Init(this);
			subsystems[name] = subsystem;
			if (name == SubSystemConstants.ConversionManagerKey)
				ConversionSubSystem = (IConversionManager) subsystem;
			else if (name == SubSystemConstants.NamingKey)
				NamingSubSystem = (INamingSubSystem) subsystem;
		}

		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			var result = NamingSubSystem.GetAssignableHandlers(service);

			// If a parent kernel exists, we merge both results
			if (Parent != null)
			{
				var parentResult = Parent.GetAssignableHandlers(service);

				if (parentResult.Length > 0)
				{
					var newResult = new IHandler[result.Length + parentResult.Length];
					result.CopyTo(newResult, 0);
					parentResult.CopyTo(newResult, result.Length);
					result = newResult;
				}
			}

			return result;
		}

		public virtual IFacility[] GetFacilities()
		{
			return facilities.ToArray();
		}

		public virtual IHandler GetHandler(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			var handler = NamingSubSystem.GetHandler(name);

			if (handler == null && Parent != null)
				handler = WrapParentHandler(Parent.GetHandler(name));

			return handler;
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null)
				throw new ArgumentNullException("service");

			var handler = NamingSubSystem.GetHandler(service);
			if (handler == null && Parent != null)
				handler = WrapParentHandler(Parent.GetHandler(service));

			return handler;
		}

		public virtual IHandler[] GetHandlers(Type service)
		{
			var result = NamingSubSystem.GetHandlers(service);

			// If a parent kernel exists, we merge both results
			if (Parent != null)
			{
				var parentResult = Parent.GetHandlers(service);

				if (parentResult.Length > 0)
				{
					var newResult = new IHandler[result.Length + parentResult.Length];
					result.CopyTo(newResult, 0);
					parentResult.CopyTo(newResult, result.Length);
					result = newResult;
				}
			}

			return result;
		}

		public virtual ISubSystem GetSubSystem(string name)
		{
			ISubSystem subsystem;
			subsystems.TryGetValue(name, out subsystem);
			return subsystem;
		}

		public virtual bool HasComponent(string name)
		{
			if (name == null)
				return false;

			if (NamingSubSystem.Contains(name))
				return true;

			if (Parent != null)
				return Parent.HasComponent(name);

			return false;
		}

		public virtual bool HasComponent(Type serviceType)
		{
			if (serviceType == null)
				return false;

			if (NamingSubSystem.Contains(serviceType))
				return true;

			if (Parent != null)
				return Parent.HasComponent(serviceType);

			return false;
		}

		public IKernel Register(params IRegistration[] registrations)
		{
			if (registrations == null)
				throw new ArgumentNullException("registrations");

			var token = OptimizeDependencyResolution();
			foreach (var registration in registrations)
				registration.Register(this);
			if (token != null)
				token.Dispose();
			return this;
		}

		public virtual void ReleaseComponent(object instance)
		{
			if (ReleasePolicy.HasTrack(instance))
			{
				ReleasePolicy.Release(instance);
			}
			else
			{
				if (Parent != null)
					Parent.ReleaseComponent(instance);
			}
		}

		public virtual void RemoveChildKernel(IKernel childKernel)
		{
			if (childKernel == null)
				throw new ArgumentNullException("childKernel");

			childKernel.Parent = null;
			childKernels.Remove(childKernel);
		}

		public virtual IHandler AddCustomComponent(ComponentModel model)
		{
			var handler = (this as IKernelInternal).CreateHandler(model);
			NamingSubSystem.Register(handler);
			(this as IKernelInternal).RaiseEventsOnHandlerCreated(handler);
			return handler;
		}

		IHandler IKernelInternal.CreateHandler(ComponentModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			RaiseComponentModelCreated(model);
			return HandlerFactory.Create(model);
		}

		public ILogger Logger { get; set; }

		public virtual ILifestyleManager CreateLifestyleManager(ComponentModel model, IComponentActivator activator)
		{
			ILifestyleManager manager;
			var type = model.LifestyleType;

			switch (type)
			{
                // This is not a thing anymore
				//case LifestyleType.Scoped:
				//	manager = new ScopedLifestyleManager(CreateScopeAccessor(model));
				//	break;
				case LifestyleType.Bound:
					manager = new ScopedLifestyleManager(CreateScopeAccessorForBoundLifestyle(model));
					break;
				case LifestyleType.Thread:
					manager = new ScopedLifestyleManager(new ThreadScopeAccessor());
					break;
				case LifestyleType.Transient:
					manager = new TransientLifestyleManager();
					break;
                case LifestyleType.PerWebRequest:
                    manager = new ScopedLifestyleManager(new WebRequestScopeAccessor());
                    break;
                case LifestyleType.Custom:
					manager = model.CustomLifestyle.CreateInstance<ILifestyleManager>();

					break;
				case LifestyleType.Pooled:
					var initial = ExtendedPropertiesConstants.Pool_Default_InitialPoolSize;
					var maxSize = ExtendedPropertiesConstants.Pool_Default_MaxPoolSize;

					if (model.ExtendedProperties.Contains(ExtendedPropertiesConstants.Pool_InitialPoolSize))
						initial = (int) model.ExtendedProperties[ExtendedPropertiesConstants.Pool_InitialPoolSize];
					if (model.ExtendedProperties.Contains(ExtendedPropertiesConstants.Pool_MaxPoolSize))
						maxSize = (int) model.ExtendedProperties[ExtendedPropertiesConstants.Pool_MaxPoolSize];

					manager = new PoolableLifestyleManager(initial, maxSize);
					break;
				default:
					//this includes LifestyleType.Undefined, LifestyleType.Singleton and invalid values
					manager = new SingletonLifestyleManager();
					break;
			}

			manager.Init(activator, this, model);

			return manager;
		}

		public virtual IComponentActivator CreateComponentActivator(ComponentModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			IComponentActivator activator;

			if (model.CustomComponentActivator == null)
				activator = new DefaultComponentActivator(model, this,
					RaiseComponentCreated,
					RaiseComponentDestroyed);
			else
				try
				{
					activator = model.CustomComponentActivator.CreateInstance<IComponentActivator>(model, this, new ComponentInstanceDelegate(RaiseComponentCreated), new ComponentInstanceDelegate(RaiseComponentDestroyed));
				}
				catch (Exception e)
				{
					throw new KernelException("Could not instantiate custom activator", e);
				}

			return activator;
		}

		void IKernelInternal.RaiseEventsOnHandlerCreated(IHandler handler)
		{
			RaiseHandlerRegistered(handler);
			RaiseHandlersChanged();
			RaiseComponentRegistered(handler.ComponentModel.Name, handler);
		}

		IHandler IKernelInternal.LoadHandlerByName(string name, Type service, IDictionary arguments)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			var handler = GetHandler(name);
			if (handler != null)
				return handler;
			lock (lazyLoadingLock)
			{
				handler = GetHandler(name);
				if (handler != null)
					return handler;

				if (isCheckingLazyLoaders)
					return null;

				isCheckingLazyLoaders = true;
				try
				{
					foreach (var loader in ResolveAll<ILazyComponentLoader>())
					{
						var registration = loader.Load(name, service, arguments);
						if (registration != null)
						{
							registration.Register(this);
							return GetHandler(name);
						}
					}
					return null;
				}
				finally
				{
					isCheckingLazyLoaders = false;
				}
			}
		}

		IHandler IKernelInternal.LoadHandlerByType(string name, Type service, IDictionary arguments)
		{
			if (service == null)
				throw new ArgumentNullException("service");

			var handler = GetHandler(service);
			if (handler != null)
				return handler;

			lock (lazyLoadingLock)
			{
				handler = GetHandler(service);
				if (handler != null)
					return handler;

				if (isCheckingLazyLoaders)
					return null;

				isCheckingLazyLoaders = true;
				try
				{
					foreach (var loader in ResolveAll<ILazyComponentLoader>())
					{
						var registration = loader.Load(name, service, arguments);
						if (registration != null)
						{
							registration.Register(this);
							return GetHandler(service);
						}
					}
					return null;
				}
				finally
				{
					isCheckingLazyLoaders = false;
				}
			}
		}

		private static IScopeAccessor CreateScopeAccessor(ComponentModel model)
		{
			var scopeAccessorType = model.GetScopeAccessorType();
			return scopeAccessorType.CreateInstance<IScopeAccessor>();
		}

		private IScopeAccessor CreateScopeAccessorForBoundLifestyle(ComponentModel model)
		{
			var selector = (Func<IHandler[], IHandler>) model.ExtendedProperties[Constants.ScopeRootSelector];
			if (selector == null)
				throw new ComponentRegistrationException(
					string.Format("Component {0} has lifestyle {1} but it does not specify mandatory 'scopeRootSelector'.",
						model.Name, LifestyleType.Bound));

			return new CreationContextScopeAccessor(model, selector);
		}

		protected CreationContext CreateCreationContext(IHandler handler, Type requestedType, IDictionary additionalArguments, CreationContext parent,
			IReleasePolicy policy)
		{
			return new CreationContext(handler, policy, requestedType, additionalArguments, ConversionSubSystem, parent);
		}

		protected void DisposeHandler(IHandler handler)
		{
			var disposable = handler as IDisposable;
			if (disposable == null)
				return;

			disposable.Dispose();
		}

		protected virtual void RegisterSubSystems()
		{
			AddSubSystem(SubSystemConstants.ConfigurationStoreKey,
				new DefaultConfigurationStore());

			AddSubSystem(SubSystemConstants.ConversionManagerKey,
				new DefaultConversionManager());

			AddSubSystem(SubSystemConstants.NamingKey,
				new DefaultNamingSubSystem());

			AddSubSystem(SubSystemConstants.ResourceKey,
				new DefaultResourceSubSystem());

			AddSubSystem(SubSystemConstants.DiagnosticsKey,
				new DefaultDiagnosticsSubSystem());
		}

		protected object ResolveComponent(IHandler handler, Type service, IDictionary additionalArguments, IReleasePolicy policy)
		{
			Debug.Assert(handler != null, "handler != null");
			var parent = currentCreationContext;
			var context = CreateCreationContext(handler, service, additionalArguments, parent, policy);
			currentCreationContext = context;

			try
			{
				return handler.Resolve(context);
			}
			finally
			{
				currentCreationContext = parent;
			}
		}

		protected virtual IHandler WrapParentHandler(IHandler parentHandler)
		{
			if (parentHandler == null)
				return null;

			var handler = new ParentHandlerWrapper(parentHandler, Parent.Resolver, Parent.ReleasePolicy);
			handler.Init(this);
			return handler;
		}

		private void DisposeComponentsInstancesWithinTracker()
		{
			ReleasePolicy.Dispose();
		}

		private void DisposeHandlers()
		{
			var vertices = TopologicalSortAlgo.Sort(GraphNodes);

			for (var i = 0; i < vertices.Length; i++)
			{
				var model = (ComponentModel) vertices[i];
				var handler = NamingSubSystem.GetHandler(model.Name);
				DisposeHandler(handler);
			}
		}

		private void DisposeSubKernels()
		{
			foreach (var childKernel in childKernels)
				childKernel.Dispose();
		}

		private void HandlerRegisteredOnParentKernel(IHandler handler, ref bool stateChanged)
		{
			RaiseHandlerRegistered(handler);
		}

		private void HandlersChangedOnParentKernel(ref bool changed)
		{
			RaiseHandlersChanged();
		}

		private void SubscribeToParentKernel()
		{
			if (parentKernel == null)
				return;

			parentKernel.HandlerRegistered += HandlerRegisteredOnParentKernel;
			parentKernel.HandlersChanged += HandlersChangedOnParentKernel;
			parentKernel.ComponentRegistered += RaiseComponentRegistered;
		}

		private void TerminateFacilities()
		{
			foreach (var facility in facilities)
				facility.Terminate();
		}

		private void UnsubscribeFromParentKernel()
		{
			if (parentKernel == null)
				return;

			parentKernel.HandlerRegistered -= HandlerRegisteredOnParentKernel;
			parentKernel.HandlersChanged -= HandlersChangedOnParentKernel;
			parentKernel.ComponentRegistered -= RaiseComponentRegistered;
		}
	}
}