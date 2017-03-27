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
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

namespace Castle.MicroKernel
{
	public partial interface IKernel : IKernelEvents, IDisposable
	{
		IComponentModelBuilder ComponentModelBuilder { get; }

		IConfigurationStore ConfigurationStore { get; set; }

		GraphNode[] GraphNodes { get; }

		IHandlerFactory HandlerFactory { get; }

		IKernel Parent { get; set; }

		IProxyFactory ProxyFactory { get; set; }

		IReleasePolicy ReleasePolicy { get; set; }

		IDependencyResolver Resolver { get; }

		void AddChildKernel(IKernel kernel);

		IKernel AddFacility(IFacility facility);

		IKernel AddFacility<T>() where T : IFacility, new();

		IKernel AddFacility<T>(Action<T> onCreate)
			where T : IFacility, new();

		void AddHandlerSelector(IHandlerSelector selector);

		void AddHandlersFilter(IHandlersFilter filter);

		void AddSubSystem(string name, ISubSystem subsystem);

		IHandler[] GetAssignableHandlers(Type service);

		IFacility[] GetFacilities();

		IHandler GetHandler(string name);

		IHandler GetHandler(Type service);

		IHandler[] GetHandlers(Type service);

		ISubSystem GetSubSystem(string name);

		bool HasComponent(string name);

		bool HasComponent(Type service);

		IKernel Register(params IRegistration[] registrations);

		void ReleaseComponent(object instance);

		void RemoveChildKernel(IKernel kernel);
	}
}