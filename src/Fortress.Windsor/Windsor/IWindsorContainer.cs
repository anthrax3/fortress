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
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Castle.Windsor
{
	public interface IWindsorContainer : IDisposable
	{
		IKernel Kernel { get; }

		string Name { get; }

		IWindsorContainer Parent { get; set; }

		void AddChildContainer(IWindsorContainer childContainer);

		IWindsorContainer AddFacility(IFacility facility);

		IWindsorContainer AddFacility<TFacility>() where TFacility : IFacility, new();

		IWindsorContainer AddFacility<TFacility>(Action<TFacility> onCreate)
			where TFacility : IFacility, new();

		IWindsorContainer GetChildContainer(string name);

		IWindsorContainer Install(params IWindsorInstaller[] installers);

		IWindsorContainer Register(params IRegistration[] registrations);

		void Release(object instance);

		void RemoveChildContainer(IWindsorContainer childContainer);

		object Resolve(string key, Type service);

		object Resolve(Type service);

		object Resolve(Type service, IDictionary arguments);

		object Resolve(Type service, object argumentsAsAnonymousType);

		T Resolve<T>();

		T Resolve<T>(IDictionary arguments);

		T Resolve<T>(object argumentsAsAnonymousType);

		T Resolve<T>(string key);

		T Resolve<T>(string key, IDictionary arguments);

		T Resolve<T>(string key, object argumentsAsAnonymousType);

		object Resolve(string key, Type service, IDictionary arguments);

		object Resolve(string key, Type service, object argumentsAsAnonymousType);

		T[] ResolveAll<T>();

		Array ResolveAll(Type service);

		Array ResolveAll(Type service, IDictionary arguments);

		Array ResolveAll(Type service, object argumentsAsAnonymousType);

		T[] ResolveAll<T>(IDictionary arguments);

		T[] ResolveAll<T>(object argumentsAsAnonymousType);
	}
}