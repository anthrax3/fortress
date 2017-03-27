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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Facilities.TypedFactory.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;

namespace Castle.Facilities.TypedFactory
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TypedFactoryRegistrationExtensions
	{
		public static ComponentRegistration<TFactoryInterface> AsFactory<TFactoryInterface>(
			this ComponentRegistration<TFactoryInterface> registration)
			where TFactoryInterface : class
		{
			return AsFactory(registration, default(Action<TypedFactoryConfiguration>));
		}

		public static ComponentRegistration<TFactoryInterface> AsFactory<TFactoryInterface>(
			this ComponentRegistration<TFactoryInterface> registration, string selectorComponentName)
			where TFactoryInterface : class
		{
			return AsFactory(registration, x => x.SelectedWith(selectorComponentName));
		}

		public static ComponentRegistration<TFactoryInterface> AsFactory<TFactoryInterface>(
			this ComponentRegistration<TFactoryInterface> registration, Type selectorComponentType)
			where TFactoryInterface : class
		{
			return AsFactory(registration, x => x.SelectedWith(selectorComponentType));
		}

		public static ComponentRegistration<TFactoryInterface> AsFactory<TFactoryInterface>(
			this ComponentRegistration<TFactoryInterface> registration, ITypedFactoryComponentSelector selector)
			where TFactoryInterface : class
		{
			return AsFactory(registration, x => x.SelectedWith(selector));
		}

		public static ComponentRegistration<TFactoryInterface> AsFactory<TFactoryInterface>(
			this ComponentRegistration<TFactoryInterface> registration,
			Action<TypedFactoryConfiguration> configuration)
			where TFactoryInterface : class
		{
			if (registration == null)
				throw new ArgumentNullException("registration");
			var classServices = registration.Services.TakeWhile(s => s.GetTypeInfo().IsClass).ToArray();
			if (classServices.Any() == false)
			{
				Debug.Assert(registration.Services.Any(), "registration.Services.Any()");
				return RegisterInterfaceBasedFactory(registration, configuration);
			}
			if (classServices.Length != 1)
				throw new ComponentRegistrationException(
					"This component can not be used as typed factory because it exposes more than one class service. Only component exposing single depegate, or interfaces can be used as typed factories.");
			var classService = classServices[0];
			if (classService.GetTypeInfo().BaseType == typeof(MulticastDelegate))
			{
				if (registration.ServicesCount == 1) // the delegate is the only service we expose
					return RegisterDelegateBasedFactory(registration, configuration, classService);
				throw new ComponentRegistrationException(
					string.Format(
						"Type {0} is a delegate, however the component has also {1} inteface(s) specified as its service. Delegate-based typed factories can't expose any additional services.",
						classService.Name, registration.ServicesCount - 1));
			}

			throw new ComponentRegistrationException(
				string.Format(
					"Type {0} is not an interface nor a delegate. Only interfaces and delegates may be used as typed factories.",
					classService.Name));
		}

		private static ComponentRegistration<TFactory> AttachConfiguration<TFactory>(
			ComponentRegistration<TFactory> componentRegistration,
			Action<TypedFactoryConfiguration> configuration,
			string defaultComponentSelectorKey)
			where TFactory : class
		{
			var selectorReference = GetSelectorReference(configuration, defaultComponentSelectorKey, typeof(TFactory));

			return componentRegistration
				.AddDescriptor(new ReferenceDependencyDescriptor(selectorReference))
				.DynamicParameters((k, c, d) =>
				{
					var selector = selectorReference.Resolve(k, c);
					d.InsertTyped(selector);
					return k2 => k2.ReleaseComponent(selector);
				})
				.AddAttributeDescriptor(TypedFactoryFacility.IsFactoryKey, bool.TrueString);
		}

		private static ComponentRegistration<TFactory> AttachFactoryInterceptor<TFactory>(
			ComponentRegistration<TFactory> registration)
			where TFactory : class
		{
			return registration.Interceptors(new InterceptorReference(TypedFactoryFacility.InterceptorKey)).Last;
		}

		private static IReference<ITypedFactoryComponentSelector> GetSelectorReference(
			Action<TypedFactoryConfiguration> configuration,
			string defaultComponentSelectorKey, Type factoryType)
		{
			var factoryConfiguration = new TypedFactoryConfiguration(defaultComponentSelectorKey, factoryType);

			if (configuration != null)
				configuration.Invoke(factoryConfiguration);
			return factoryConfiguration.Reference;
		}

		private static bool HasOutArguments(Type serviceType)
		{
			return serviceType.GetMethods().Any(m => m.GetParameters().Any(p => p.IsOut));
		}

		private static ComponentRegistration<TDelegate> RegisterDelegateBasedFactory<TDelegate>(
			ComponentRegistration<TDelegate> registration,
			Action<TypedFactoryConfiguration> configuration,
			Type delegateType)
			where TDelegate : class
		{
			if (HasOutArguments(delegateType))
				throw new ComponentRegistrationException(
					string.Format("Delegate type {0} can not be used as typed factory because it has 'out' arguments.",
						delegateType));
			var invoke = DelegateFactory.ExtractInvokeMethod(delegateType);
			if (invoke == null)
				throw new ComponentRegistrationException(
					string.Format("Delegate type {0} can not be used as typed factory because it has void return type.",
						delegateType));
			var settings = new TypedFactoryConfiguration(TypedFactoryFacility.DefaultDelegateSelectorKey, typeof(TDelegate));
			if (configuration != null)
				configuration.Invoke(settings);

			var componentRegistration = AttachFactoryInterceptor(registration)
				.Activator<DelegateFactoryActivator>();
			return AttachConfiguration(componentRegistration, configuration, TypedFactoryFacility.DefaultDelegateSelectorKey);
		}

		private static ComponentRegistration<TFactoryInterface> RegisterInterfaceBasedFactory<TFactoryInterface>(
			ComponentRegistration<TFactoryInterface> registration, Action<TypedFactoryConfiguration> configuration)
			where TFactoryInterface : class
		{
			foreach (var serviceType in registration.Services)
			{
				Debug.Assert(serviceType.GetTypeInfo().IsInterface, "serviceType.IsInterface");
				if (HasOutArguments(serviceType))
					throw new ComponentRegistrationException(
						string.Format("Type {0} can not be used as typed factory because it has methods with 'out' arguments.",
							serviceType));
			}
			var componentRegistration = AttachFactoryInterceptor(registration);
			return AttachConfiguration(componentRegistration, configuration, TypedFactoryFacility.DefaultInterfaceSelectorKey);
		}
	}
}