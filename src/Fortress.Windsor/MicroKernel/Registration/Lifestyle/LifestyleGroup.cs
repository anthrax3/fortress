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
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.ModelBuilder.Descriptors;

namespace Castle.MicroKernel.Registration.Lifestyle
{
	public class LifestyleGroup<TService> : RegistrationGroup<TService>
		where TService : class
	{
		public LifestyleGroup(ComponentRegistration<TService> registration)
			: base(registration)
		{
		}

		public ComponentRegistration<TService> Transient
		{
			get { return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Transient)); }
		}

		public ComponentRegistration<TService> Singleton
		{
			get { return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Singleton)); }
		}

		public ComponentRegistration<TService> PerThread
		{
			get { return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Thread)); }
		}

		public ComponentRegistration<TService> PerWebRequest
		{
			get { return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.PerWebRequest)); }
		}

		public ComponentRegistration<TService> Pooled
		{
			get { return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Pooled)); }
		}

		public ComponentRegistration<TService> Is(LifestyleType type)
		{
			if (Enum.IsDefined(typeof(LifestyleType), type) == false)
				throw InvalidValue(type, "Not a valid lifestyle");
			if (type == LifestyleType.Undefined)
				throw InvalidValue(type, string.Format("{0} is not a valid lifestyle type.", LifestyleType.Undefined));

			return AddDescriptor(new LifestyleDescriptor<TService>(type));
		}

		private ArgumentOutOfRangeException InvalidValue(LifestyleType type, string message)
		{
			return new ArgumentOutOfRangeException("type", type, message);
		}

		public ComponentRegistration<TService> PooledWithSize(int? initialSize, int? maxSize)
		{
			var pooledWithSize = Pooled;
			if (initialSize.HasValue)
				pooledWithSize = pooledWithSize.Attribute("initialPoolSize").Eq(initialSize);
			if (maxSize.HasValue)
				pooledWithSize = pooledWithSize.Attribute("maxPoolSize").Eq(maxSize);
			return pooledWithSize;
		}

		//public ComponentRegistration<TService> Scoped<TScopeAccessor>() where TScopeAccessor : IScopeAccessor, new()
		//{
		//	return Scoped(typeof(TScopeAccessor));
		//}

		//public ComponentRegistration<TService> Scoped(Type scopeAccessorType)
		//{
		//	var registration = AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Scoped));
		//	if (scopeAccessorType == null)
		//		return registration;
		//	var scopeAccessor = new ExtendedPropertiesDescriptor(new Property(Constants.ScopeAccessorType, scopeAccessorType));
		//	return registration.AddDescriptor(scopeAccessor);
		//}

		//public ComponentRegistration<TService> Scoped()
		//{
		//	return Scoped(null);
		//}

		public ComponentRegistration<TService> BoundTo<TBaseForRoot>() where TBaseForRoot : class
		{
			return BoundTo(CreationContextScopeAccessor.DefaultScopeRootSelector<TBaseForRoot>);
		}

		public ComponentRegistration<TService> BoundToNearest<TBaseForRoot>() where TBaseForRoot : class
		{
			return BoundTo(CreationContextScopeAccessor.NearestScopeRootSelector<TBaseForRoot>);
		}

		public ComponentRegistration<TService> BoundTo(Func<IHandler[], IHandler> scopeRootBinder)
		{
			return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Bound))
				.ExtendedProperties(new Property(Constants.ScopeRootSelector, scopeRootBinder));
		}

		public ComponentRegistration<TService> Custom(Type customLifestyleType)
		{
			if (customLifestyleType.Is<ILifestyleManager>() == false)
				throw new ComponentRegistrationException(string.Format(
					"The type {0} must implement {1} to " +
					"be used as a custom lifestyle", customLifestyleType.FullName, typeof(ILifestyleManager).FullName));

			return AddDescriptor(new LifestyleDescriptor<TService>(LifestyleType.Custom))
				.Attribute("customLifestyleType").Eq(customLifestyleType.AssemblyQualifiedName);
		}

		public ComponentRegistration<TService> Custom<TLifestyleManager>()
			where TLifestyleManager : ILifestyleManager, new()
		{
			return Custom(typeof(TLifestyleManager));
		}
	}
}