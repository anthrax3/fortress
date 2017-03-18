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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;

	public partial interface IKernel : IKernelEvents, IDisposable
	{
		object Resolve(Type service);

		object Resolve(Type service, IDictionary arguments);

		object Resolve(Type service, object argumentsAsAnonymousType);

		object Resolve(String key, Type service);

		T Resolve<T>(IDictionary arguments);

		T Resolve<T>(object argumentsAsAnonymousType);

		T Resolve<T>();

		T Resolve<T>(String key);

		T Resolve<T>(String key, IDictionary arguments);

		object Resolve(String key, Type service, IDictionary arguments);

		Array ResolveAll(Type service);

		Array ResolveAll(Type service, IDictionary arguments);

		Array ResolveAll(Type service, object argumentsAsAnonymousType);

		TService[] ResolveAll<TService>();

		TService[] ResolveAll<TService>(IDictionary arguments);

		TService[] ResolveAll<TService>(object argumentsAsAnonymousType);
	}
}
