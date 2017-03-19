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

namespace Castle.Windsor.MicroKernel.SubSystems.Naming
{
	public interface INamingSubSystem : ISubSystem
	{
		int ComponentCount { get; }

		void AddHandlerSelector(IHandlerSelector selector);

		void AddHandlersFilter(IHandlersFilter filter);

		bool Contains(String name);

		bool Contains(Type service);

		IHandler[] GetAllHandlers();

		IHandler[] GetAssignableHandlers(Type service);

		IHandler GetHandler(String name);

		IHandler GetHandler(Type service);

		IHandler[] GetHandlers(Type service);

		void Register(IHandler handler);
	}
}
