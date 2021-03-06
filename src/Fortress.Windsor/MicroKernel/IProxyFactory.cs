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

using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Proxy;

namespace Castle.MicroKernel
{
	public interface IProxyFactory
	{
		void AddInterceptorSelector(IModelInterceptorsSelector selector);

		object Create(IKernel kernel, object instance, ComponentModel model, CreationContext context, params object[] constructorArguments);

		object Create(IProxyFactoryExtension customFactory, IKernel kernel, ComponentModel model, CreationContext context,
			params object[] constructorArguments);

		bool RequiresTargetInstance(IKernel kernel, ComponentModel model);

		bool ShouldCreateProxy(ComponentModel model);
	}
}