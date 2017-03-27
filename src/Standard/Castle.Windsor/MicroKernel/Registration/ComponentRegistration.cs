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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.Compatibility;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.LifecycleConcerns;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration.Interceptor;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.MicroKernel.Registration.Proxy;

namespace Castle.MicroKernel.Registration
{
	public class ComponentRegistration<TService> : IRegistration
		where TService : class
	{
		private readonly List<IComponentModelDescriptor> descriptors = new List<IComponentModelDescriptor>();
		private readonly List<Type> potentialServices = new List<Type>();

		private bool ifComponentRegisteredIgnore;
		private ComponentName name;
		private bool registered;
		private bool registerNewServicesOnly;

		public ComponentRegistration() : this(typeof(TService))
		{
		}

		public ComponentRegistration(params Type[] services)
		{
			Forward(services);
		}

		public Type Implementation { get; private set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public LifestyleGroup<TService> LifeStyle
		{
			get { return new LifestyleGroup<TService>(this); }
		}

		public string Name
		{
			get
			{
				if (name == null)
					return null;
				return name.Name;
			}
		}

		public ProxyGroup<TService> Proxy
		{
			get { return new ProxyGroup<TService>(this); }
		}

		protected internal IList<Type> Services
		{
			get { return potentialServices; }
		}

		protected internal int ServicesCount
		{
			get { return potentialServices.Count; }
		}

		internal bool IsOverWrite { get; private set; }

		void IRegistration.Register(IKernelInternal kernel)
		{
			if (registered)
				return;
			registered = true;
			var services = FilterServices(kernel);
			if (services.Length == 0)
				return;

			var componentModel = kernel.ComponentModelBuilder.BuildModel(GetContributors(services));
			if (SkipRegistration(kernel, componentModel))
			{
				kernel.Logger.Info("Skipping registration of " + componentModel.Name);
				return;
			}
			kernel.AddCustomComponent(componentModel);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("If you're using WCF Facility use AsWcfClient/AsWcfService extension methods instead.")]
		public ComponentRegistration<TService> ActAs(params object[] actors)
		{
			foreach (var actor in actors)
				if (actor != null)
					DependsOn(Property.ForKey(Guid.NewGuid().ToString()).Eq(actor));
			return this;
		}

		public ComponentRegistration<TService> Activator<TActivator>() where TActivator : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(TActivator).AssemblyQualifiedName);
		}

		public ComponentRegistration<TService> AddAttributeDescriptor(string key, string value)
		{
			AddDescriptor(new AttributeDescriptor<TService>(key, value));
			return this;
		}

		public ComponentRegistration<TService> AddDescriptor(IComponentModelDescriptor descriptor)
		{
			descriptors.Add(descriptor);
			var componentDescriptor = descriptor as AbstractOverwriteableDescriptor<TService>;
			if (componentDescriptor != null)
				componentDescriptor.Registration = this;
			return this;
		}

		public AttributeKeyDescriptor<TService> Attribute(string key)
		{
			return new AttributeKeyDescriptor<TService>(this, key);
		}

		public ComponentRegistration<TService> Configuration(params Node[] configNodes)
		{
			return AddDescriptor(new ConfigurationDescriptor(configNodes));
		}

		public ComponentRegistration<TService> Configuration(IConfiguration configuration)
		{
			return AddDescriptor(new ConfigurationDescriptor(configuration));
		}

		public ComponentRegistration<TService> DependsOn(Dependency dependency)
		{
			return DependsOn(new[] {dependency});
		}

		public ComponentRegistration<TService> DependsOn(params Dependency[] dependencies)
		{
			if (dependencies == null || dependencies.Length == 0)
				return this;
			var serviceOverrides = new List<ServiceOverride>(dependencies.Length);
			var properties = new List<Property>(dependencies.Length);
			var parameters = new List<Parameter>(dependencies.Length);
			foreach (var dependency in dependencies)
			{
				if (dependency.Accept(properties))
					continue;
				if (dependency.Accept(parameters))
					continue;
				if (dependency.Accept(serviceOverrides))
					continue;
			}

			if (serviceOverrides.Count > 0)
				AddDescriptor(new ServiceOverrideDescriptor(serviceOverrides.ToArray()));
			if (properties.Count > 0)
				AddDescriptor(new CustomDependencyDescriptor(properties.ToArray()));

			if (parameters.Count > 0)
				AddDescriptor(new ParametersDescriptor(parameters.ToArray()));
			return this;
		}

		public ComponentRegistration<TService> DependsOn(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor(dependencies));
		}

		public ComponentRegistration<TService> DependsOn(object dependenciesAsAnonymousType)
		{
			return AddDescriptor(new CustomDependencyDescriptor(new ReflectionBasedDictionaryAdapter(dependenciesAsAnonymousType)));
		}

		public ComponentRegistration<TService> DependsOn(DynamicParametersDelegate resolve)
		{
			return DynamicParameters((k, c, d) =>
			{
				resolve(k, d);
				return null;
			});
		}

		public ComponentRegistration<TService> DependsOn(DynamicParametersResolveDelegate resolve)
		{
			return DynamicParameters((k, c, d) => resolve(k, d));
		}

		public ComponentRegistration<TService> DependsOn(DynamicParametersWithContextResolveDelegate resolve)
		{
			AddDescriptor(new DynamicParametersDescriptor(resolve));
			return this;
		}

		public ComponentRegistration<TService> DynamicParameters(DynamicParametersDelegate resolve)
		{
			return DynamicParameters((k, c, d) =>
			{
				resolve(k, d);
				return null;
			});
		}

		public ComponentRegistration<TService> DynamicParameters(DynamicParametersResolveDelegate resolve)
		{
			return DynamicParameters((k, c, d) => resolve(k, d));
		}

		public ComponentRegistration<TService> DynamicParameters(DynamicParametersWithContextResolveDelegate resolve)
		{
			AddDescriptor(new DynamicParametersDescriptor(resolve));
			return this;
		}

		public ComponentRegistration<TService> ExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor(properties));
		}

		public ComponentRegistration<TService> ExtendedProperties(Property property)
		{
			return ExtendedProperties(new[] {property});
		}

		public ComponentRegistration<TService> ExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor(new ReflectionBasedDictionaryAdapter(anonymous)));
		}

		public ComponentRegistration<TService> Forward(params Type[] types)
		{
			return Forward((IEnumerable<Type>) types);
		}

		public ComponentRegistration<TService> Forward<TService2>()
		{
			return Forward(typeof(TService2));
		}

		public ComponentRegistration<TService> Forward<TService2, TService3>()
		{
			return Forward(typeof(TService2), typeof(TService3));
		}

		public ComponentRegistration<TService> Forward<TService2, TService3, TService4>()
		{
			return Forward(typeof(TService2), typeof(TService3), typeof(TService4));
		}

		public ComponentRegistration<TService> Forward<TService2, TService3, TService4, TService5>()
		{
			return Forward(typeof(TService2), typeof(TService3), typeof(TService4), typeof(TService5));
		}

		public ComponentRegistration<TService> Forward(IEnumerable<Type> types)
		{
			foreach (var type in types)
				ComponentServicesUtil.AddService(potentialServices, type);
			return this;
		}

		public ComponentRegistration<TService> ImplementedBy<TImpl>() where TImpl : TService
		{
			return ImplementedBy(typeof(TImpl));
		}

		public ComponentRegistration<TService> ImplementedBy(Type type)
		{
			return ImplementedBy(type, null, null);
		}

		public ComponentRegistration<TService> ImplementedBy(Type type, IGenericImplementationMatchingStrategy genericImplementationMatchingStrategy)
		{
			return ImplementedBy(type, genericImplementationMatchingStrategy, null);
		}

		public ComponentRegistration<TService> ImplementedBy(Type type, IGenericServiceStrategy genericServiceStrategy)
		{
			return ImplementedBy(type, null, genericServiceStrategy);
		}

		public ComponentRegistration<TService> ImplementedBy(Type type, IGenericImplementationMatchingStrategy genericImplementationMatchingStrategy, IGenericServiceStrategy genericServiceStrategy)
		{
			if (Implementation != null && Implementation != typeof(LateBoundComponent))
			{
				var message = string.Format("This component has already been assigned implementation {0}",
					Implementation.FullName);
				throw new ComponentRegistrationException(message);
			}

			Implementation = type;
			if (genericImplementationMatchingStrategy != null)
				ExtendedProperties(Property.ForKey(Constants.GenericImplementationMatchingStrategy).Eq(genericImplementationMatchingStrategy));
			if (genericServiceStrategy != null)
				ExtendedProperties(Property.ForKey(Constants.GenericServiceStrategy).Eq(genericServiceStrategy));
			return this;
		}

		public ComponentRegistration<TService> Instance(TService instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			return ImplementedBy(instance.GetType())
				.Activator<ExternalInstanceActivator>()
				.ExtendedProperties(Property.ForKey("instance").Eq(instance));
		}

		public InterceptorGroup<TService> Interceptors(params InterceptorReference[] interceptors)
		{
			return new InterceptorGroup<TService>(this, interceptors);
		}

		public ComponentRegistration<TService> Interceptors(params Type[] interceptors)
		{
			var references = interceptors.ConvertAll(t => new InterceptorReference(t));
			return AddDescriptor(new InterceptorDescriptor(references));
		}

		public ComponentRegistration<TService> Interceptors<TInterceptor>() where TInterceptor : IInterceptor
		{
			return AddDescriptor(new InterceptorDescriptor(new[] {new InterceptorReference(typeof(TInterceptor))}));
		}

		public ComponentRegistration<TService> Interceptors<TInterceptor1, TInterceptor2>()
			where TInterceptor1 : IInterceptor
			where TInterceptor2 : IInterceptor
		{
			return Interceptors<TInterceptor1>().Interceptors<TInterceptor2>();
		}

		public ComponentRegistration<TService> Interceptors(params string[] keys)
		{
			var interceptors = keys.ConvertAll(InterceptorReference.ForKey);
			return AddDescriptor(new InterceptorDescriptor(interceptors));
		}

		public ComponentRegistration<TService> LifestyleCustom(Type customLifestyleType)
		{
			return LifeStyle.Custom(customLifestyleType);
		}

		public ComponentRegistration<TService> LifestyleCustom<TLifestyleManager>()
			where TLifestyleManager : ILifestyleManager, new()
		{
			return LifeStyle.Custom<TLifestyleManager>();
		}

		public ComponentRegistration<TService> LifestylePerThread()
		{
			return LifeStyle.PerThread;
		}

        // This is not a thing anymore
		//public ComponentRegistration<TService> LifestyleScoped(Type scopeAccessorType = null)
		//{
		//	return LifeStyle.Scoped(scopeAccessorType);
		//}

        // This neither
		//public ComponentRegistration<TService> LifestyleScoped<TScopeAccessor>() where TScopeAccessor : IScopeAccessor, new()
		//{
		//	return LifestyleScoped(typeof(TScopeAccessor));
		//}

		public ComponentRegistration<TService> LifestyleBoundTo<TBaseForRoot>() where TBaseForRoot : class
		{
			return LifeStyle.BoundTo<TBaseForRoot>();
		}

		public ComponentRegistration<TService> LifestyleBoundToNearest<TBaseForRoot>() where TBaseForRoot : class
		{
			return LifeStyle.BoundToNearest<TBaseForRoot>();
		}

		public ComponentRegistration<TService> LifestyleBoundTo(Func<IHandler[], IHandler> scopeRootBinder)
		{
			return LifeStyle.BoundTo(scopeRootBinder);
		}

		public ComponentRegistration<TService> LifestylePerWebRequest()
		{
			return LifeStyle.PerWebRequest;
		}

		public ComponentRegistration<TService> LifestylePooled(int? initialSize = null, int? maxSize = null)
		{
			return LifeStyle.PooledWithSize(initialSize, maxSize);
		}

		public ComponentRegistration<TService> LifestyleSingleton()
		{
			return LifeStyle.Singleton;
		}

		public ComponentRegistration<TService> LifestyleTransient()
		{
			return LifeStyle.Transient;
		}

		public ComponentRegistration<TService> Named(string name)
		{
			if (this.name != null)
			{
				var message = string.Format("This component has already been assigned name '{0}'", this.name.Name);
				throw new ComponentRegistrationException(message);
			}
			if (name == null)
				return this;

			this.name = new ComponentName(name, true);
			return this;
		}

		public ComponentRegistration<TService> NamedAutomatically(string name)
		{
			if (this.name != null)
			{
				var message = string.Format("This component has already been assigned name '{0}'", this.name);
				throw new ComponentRegistrationException(message);
			}

			this.name = new ComponentName(name, false);
			return this;
		}

		public ComponentRegistration<TService> OnCreate(params Action<TService>[] actions)
		{
			if (actions.IsNullOrEmpty())
				return this;
			return OnCreate(actions.ConvertAll(a => new LifecycleActionDelegate<TService>((_, o) => a(o))));
		}

		public ComponentRegistration<TService> OnCreate(params LifecycleActionDelegate<TService>[] actions)
		{
			if (actions != null && actions.Length != 0)
			{
				var action = (LifecycleActionDelegate<TService>) Delegate.Combine(actions);
				AddDescriptor(new OnCreateComponentDescriptor<TService>(action));
			}
			return this;
		}

		public ComponentRegistration<TService> OnDestroy(params Action<TService>[] actions)
		{
			if (actions.IsNullOrEmpty())
				return this;
			return OnDestroy(actions.ConvertAll(a => new LifecycleActionDelegate<TService>((_, o) => a(o))));
		}

		public ComponentRegistration<TService> OnDestroy(params LifecycleActionDelegate<TService>[] actions)
		{
			if (actions != null && actions.Length != 0)
			{
				var action = (LifecycleActionDelegate<TService>) Delegate.Combine(actions);
				AddDescriptor(new OnDestroyComponentDescriptor<TService>(action));
			}
			return this;
		}

		public ComponentRegistration<TService> OnlyNewServices()
		{
			registerNewServicesOnly = true;
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public ComponentRegistration<TService> OverWrite()
		{
			IsOverWrite = true;
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DependsOn(Dependency.OnConfigValue()) or Dependency.OnValue() instead")]
		public ComponentRegistration<TService> Parameters(params Parameter[] parameters)
		{
			return AddDescriptor(new ParametersDescriptor(parameters));
		}

		public ComponentRegistration<TService> SelectInterceptorsWith(IInterceptorSelector selector)
		{
			return SelectInterceptorsWith(s => s.Instance(selector));
		}

		public ComponentRegistration<TService> SelectInterceptorsWith(Action<ItemRegistration<IInterceptorSelector>> selector)
		{
			var registration = new ItemRegistration<IInterceptorSelector>();
			selector.Invoke(registration);
			return AddDescriptor(new InterceptorSelectorDescriptor(registration.Item));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DependsOn(Dependency.OnComponent()) instead")]
		public ComponentRegistration<TService> ServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor(overrides));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DependsOn(Dependency.OnComponent()) instead")]
		public ComponentRegistration<TService> ServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor(overrides));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DependsOn(Dependency.OnComponent()) instead")]
		public ComponentRegistration<TService> ServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor(new ReflectionBasedDictionaryAdapter(anonymous)));
		}

		public ComponentRegistration<TService> UsingFactory<TFactory, TServiceImpl>(Converter<TFactory, TServiceImpl> factory)
			where TServiceImpl : TService
		{
			return UsingFactoryMethod(kernel => factory.Invoke(kernel.Resolve<TFactory>()));
		}

		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Func<TImpl> factoryMethod,
			bool managedExternally = false)
			where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod(), managedExternally);
		}

		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Converter<IKernel, TImpl> factoryMethod,
			bool managedExternally = false)
			where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod(k), managedExternally);
		}

		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(
			Func<IKernel, ComponentModel, CreationContext, TImpl> factoryMethod,
			bool managedExternally = false)
			where TImpl : TService
		{
			Activator<FactoryMethodActivator<TImpl>>()
				.ExtendedProperties(Property.ForKey("factoryMethodDelegate").Eq(factoryMethod));

			if (managedExternally)
				ExtendedProperties(Property.ForKey("factory.managedExternally").Eq(managedExternally));

			if (Implementation == null &&
			    (potentialServices.First().GetTypeInfo().IsClass == false || potentialServices.First().GetTypeInfo().IsSealed == false))
				Implementation = typeof(LateBoundComponent);
			return this;
		}

		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Func<IKernel, CreationContext, TImpl> factoryMethod)
			where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod(k, c));
		}

		internal void RegisterOptionally()
		{
			ifComponentRegisteredIgnore = true;
		}

		private Type[] FilterServices(IKernel kernel)
		{
			var services = new List<Type>(potentialServices);
			if (registerNewServicesOnly)
				services.RemoveAll(kernel.HasComponent);
			return services.ToArray();
		}

		private IComponentModelDescriptor[] GetContributors(Type[] services)
		{
			var list = new List<IComponentModelDescriptor>
			{
				new ServicesDescriptor(services),
				new DefaultsDescriptor(name, Implementation)
			};
			list.AddRange(descriptors);
			return list.ToArray();
		}

		private bool SkipRegistration(IKernelInternal internalKernel, ComponentModel componentModel)
		{
			return ifComponentRegisteredIgnore && internalKernel.HasComponent(componentModel.Name);
		}

		public ComponentRegistration<TService> IsDefault(Predicate<Type> serviceFilter)
		{
			if (serviceFilter == null)
				throw new ArgumentNullException("serviceFilter");
			var properties = new Property(Constants.DefaultComponentForServiceFilter, serviceFilter);
			return ExtendedProperties(properties);
		}

		public ComponentRegistration<TService> IsDefault()
		{
			return IsDefault(_ => true);
		}

		public ComponentRegistration<TService> IsFallback(Predicate<Type> serviceFilter)
		{
			if (serviceFilter == null)
				throw new ArgumentNullException("serviceFilter");
			var properties = new Property(Constants.FallbackComponentForServiceFilter, serviceFilter);
			return ExtendedProperties(properties);
		}

		public ComponentRegistration<TService> IsFallback()
		{
			return IsFallback(_ => true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is now obsolete due to poor usability. Use explicit PropertiesRequire() or PropertiesIgnore() method instead.")]
		public ComponentRegistration<TService> Properties(Predicate<PropertyInfo> filter)
		{
			return Properties(filter, false);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is now obsolete due to poor usability. Use explicit PropertiesRequire() or PropertiesIgnore() method instead.")]
		public ComponentRegistration<TService> Properties(Predicate<PropertyInfo> filter, bool isRequired)
		{
			return Properties((_, p) => filter(p), isRequired);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is now obsolete due to poor usability. Use explicit PropertiesRequire() or PropertiesIgnore() method instead.")]
		public ComponentRegistration<TService> Properties(Func<ComponentModel, PropertyInfo, bool> filter, bool isRequired)
		{
			return AddDescriptor(new DelegatingModelDescriptor((k, c) =>
			{
				var filters = StandardPropertyFilters.GetPropertyFilters(c, true);
				filters.Add(StandardPropertyFilters.FromObsoleteFunction(filter, isRequired));
			}));
		}

		public ComponentRegistration<TService> PropertiesIgnore(Func<PropertyInfo, bool> propertySelector)
		{
			return PropertiesIgnore((_, p) => propertySelector(p));
		}

		public ComponentRegistration<TService> PropertiesRequire(Func<PropertyInfo, bool> propertySelector)
		{
			return PropertiesRequire((_, p) => propertySelector(p));
		}

		public ComponentRegistration<TService> PropertiesIgnore(Func<ComponentModel, PropertyInfo, bool> propertySelector)
		{
			return AddDescriptor(new DelegatingModelDescriptor((k, c) =>
			{
				var filters = StandardPropertyFilters.GetPropertyFilters(c, true);
				filters.Add(StandardPropertyFilters.IgnoreSelected(propertySelector));
			}));
		}

		public ComponentRegistration<TService> PropertiesRequire(Func<ComponentModel, PropertyInfo, bool> propertySelector)
		{
			return AddDescriptor(new DelegatingModelDescriptor((k, c) =>
			{
				var filters = StandardPropertyFilters.GetPropertyFilters(c, true);
				filters.Add(StandardPropertyFilters.RequireSelected(propertySelector));
			}));
		}

		public ComponentRegistration<TService> Properties(PropertyFilter filter)
		{
			return AddDescriptor(new DelegatingModelDescriptor((k, c) =>
			{
				var filters = StandardPropertyFilters.GetPropertyFilters(c, true);
				filters.Add(StandardPropertyFilters.Create(filter));
			}));
		}
	}
}