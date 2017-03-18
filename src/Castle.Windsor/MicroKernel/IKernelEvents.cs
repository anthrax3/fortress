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

	using Castle.Core;
	using Castle.Windsor;

	public delegate void ComponentDataDelegate(String key, IHandler handler);

	public delegate void ComponentInstanceDelegate(ComponentModel model, object instance);

	public delegate void ComponentModelDelegate(ComponentModel model);

	public delegate void ServiceDelegate(Type service);

	public delegate void HandlerDelegate(IHandler handler, ref bool stateChanged);

	public delegate void HandlersChangedDelegate(ref bool stateChanged);

	public delegate void DependencyDelegate(ComponentModel client, DependencyModel model, Object dependency);

	public interface IKernelEvents
	{
		event ComponentDataDelegate ComponentRegistered;

		event ComponentModelDelegate ComponentModelCreated;

		event EventHandler AddedAsChildKernel;

		event EventHandler RemovedAsChildKernel;

		event ComponentInstanceDelegate ComponentCreated;

		event ComponentInstanceDelegate ComponentDestroyed;

		event HandlerDelegate HandlerRegistered;

		event HandlersChangedDelegate HandlersChanged;

		event DependencyDelegate DependencyResolving;

		event EventHandler RegistrationCompleted;

		event ServiceDelegate EmptyCollectionResolving;
	}
}
