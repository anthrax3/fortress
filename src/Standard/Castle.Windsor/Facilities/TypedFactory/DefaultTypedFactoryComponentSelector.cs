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
using System.Reflection;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;

namespace Castle.Facilities.TypedFactory
{
	[Singleton]
	public class DefaultTypedFactoryComponentSelector : ITypedFactoryComponentSelector
	{
		public DefaultTypedFactoryComponentSelector(bool getMethodsResolveByName = true,
			bool fallbackToResolveByTypeIfNameNotFound = false)
		{
			FallbackToResolveByTypeIfNameNotFound = fallbackToResolveByTypeIfNameNotFound;
			GetMethodsResolveByName = getMethodsResolveByName;
		}

		protected DefaultTypedFactoryComponentSelector() : this(true, false)
		{
		}

		protected bool FallbackToResolveByTypeIfNameNotFound { get; set; }

		protected bool GetMethodsResolveByName { get; set; }

		public Func<IKernelInternal, IReleasePolicy, object> SelectComponent(MethodInfo method, Type type, object[] arguments)
		{
			var componentName = GetComponentName(method, arguments);
			var componentType = GetComponentType(method, arguments);
			var additionalArguments = GetArguments(method, arguments);

			return BuildFactoryComponent(method, componentName, componentType, additionalArguments);
		}

		protected virtual Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method,
			string componentName,
			Type componentType,
			IDictionary additionalArguments)
		{
			var itemType = componentType.GetCompatibleArrayItemType();
			if (itemType == null)
				return new TypedFactoryComponentResolver(componentName,
					componentType,
					additionalArguments,
					FallbackToResolveByTypeIfNameNotFound, GetType()).Resolve;
			return (k, s) => k.ResolveAll(itemType, additionalArguments, s);
		}

		protected virtual IDictionary GetArguments(MethodInfo method, object[] arguments)
		{
			if (arguments == null)
				return null;
			var argumentMap = new Arguments();
			var parameters = method.GetParameters();
			for (var i = 0; i < parameters.Length; i++)
				argumentMap.Add(parameters[i].Name, arguments[i]);
			return argumentMap;
		}

		protected virtual string GetComponentName(MethodInfo method, object[] arguments)
		{
			string componentName = null;
			if (GetMethodsResolveByName && method.Name.StartsWith("Get", StringComparison.OrdinalIgnoreCase))
				componentName = method.Name.Substring("Get".Length);
			return componentName;
		}

		protected virtual Type GetComponentType(MethodInfo method, object[] arguments)
		{
			return method.ReturnType;
		}
	}
}