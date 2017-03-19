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
using Castle.Windsor.Core;

namespace Castle.Windsor.MicroKernel.Registration
{
	public static class Component
	{
		public static ComponentRegistration For(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException("serviceType",
					"The argument was null. Check that the assembly "
					+ "is referenced and the type available to your application.");

			return new ComponentRegistration(serviceType);
		}

		public static ComponentRegistration For(params Type[] serviceTypes)
		{
			if (serviceTypes.Length == 0)
				throw new ArgumentException("At least one service type must be supplied");
			return new ComponentRegistration(serviceTypes);
		}

		public static ComponentRegistration For(IEnumerable<Type> serviceTypes)
		{
			ComponentRegistration registration = null;

			foreach (var serviceType in serviceTypes)
				if (registration == null)
					registration = For(serviceType);
				else
					registration.Forward(serviceType);

			if (registration == null)
				throw new ArgumentException("At least one service type must be supplied");

			return registration;
		}

		public static ComponentRegistration<TService> For<TService>()
			where TService : class
		{
			return new ComponentRegistration<TService>();
		}

		public static ComponentRegistration<TService1> For<TService1, TService2>()
			where TService1 : class
		{
			return new ComponentRegistration<TService1>().Forward<TService2>();
		}

		public static ComponentRegistration<TService1> For<TService1, TService2, TService3>()
			where TService1 : class
		{
			return new ComponentRegistration<TService1>().Forward<TService2, TService3>();
		}

		public static ComponentRegistration<TService1> For<TService1, TService2, TService3, TService4>()
			where TService1 : class
		{
			return new ComponentRegistration<TService1>().Forward<TService2, TService3, TService4>();
		}

		public static ComponentRegistration<TService1> For<TService1, TService2, TService3, TService4, TService5>()
			where TService1 : class
		{
			return new ComponentRegistration<TService1>().Forward<TService2, TService3, TService4, TService5>();
		}

		public static bool HasAttribute<TAttribute>(Type type) where TAttribute : Attribute
		{
			return Attribute.IsDefined(type, typeof(TAttribute));
		}

		public static Predicate<Type> HasAttribute<TAttribute>(Predicate<TAttribute> filter) where TAttribute : Attribute
		{
			return type => HasAttribute<TAttribute>(type) &&
			               filter((TAttribute) Attribute.GetCustomAttribute(type, typeof(TAttribute)));
		}

		public static bool IsCastleComponent(Type type)
		{
			return HasAttribute<CastleComponentAttribute>(type);
		}

		public static Predicate<Type> IsInNamespace(string @namespace)
		{
			return IsInNamespace(@namespace, false);
		}

		public static Predicate<Type> IsInNamespace(string @namespace, bool includeSubnamespaces)
		{
			if (includeSubnamespaces)
				return type => type.Namespace == @namespace ||
				               type.Namespace != null &&
				               type.Namespace.StartsWith(@namespace + ".");

			return type => type.Namespace == @namespace;
		}

		public static Predicate<Type> IsInSameNamespaceAs(Type type)
		{
			return IsInNamespace(type.Namespace);
		}

		public static Predicate<Type> IsInSameNamespaceAs(Type type, bool includeSubnamespaces)
		{
			return IsInNamespace(type.Namespace, includeSubnamespaces);
		}

		public static Predicate<Type> IsInSameNamespaceAs<T>()
		{
			return IsInSameNamespaceAs(typeof(T));
		}

		public static Predicate<Type> IsInSameNamespaceAs<T>(bool includeSubnamespaces)
		{
			return IsInSameNamespaceAs(typeof(T), includeSubnamespaces);
		}
	}
}