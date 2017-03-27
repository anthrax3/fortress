// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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
using System.Reflection;
using Castle.Core.Core.Internal;
using Castle.Core.DynamicProxy.Generators;
using Castle.Core.DynamicProxy.Internal;
using Castle.Core.Logging;

namespace Castle.Core.DynamicProxy
{
	public class DefaultProxyBuilder : IProxyBuilder
	{
		public DefaultProxyBuilder()
			: this(new ModuleScope())
		{
		}

		public DefaultProxyBuilder(ModuleScope scope)
		{
			ModuleScope = scope;
		}

		public ILogger Logger { get; set; } = NullLogger.Instance;

		public ModuleScope ModuleScope { get; }

		public Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new ClassProxyGenerator(ModuleScope, classToProxy) {Logger = Logger};
			return generator.GenerateCode(additionalInterfacesToProxy, options);
		}

		public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			AssertValidType(classToProxy);
			AssertValidTypes(additionalInterfacesToProxy);
			var generator = new ClassProxyWithTargetGenerator(ModuleScope, classToProxy, additionalInterfacesToProxy, options)
				{Logger = Logger};
			return generator.GetGeneratedType();
		}

		public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			Type targetType,
			ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetGenerator(ModuleScope, interfaceToProxy) {Logger = Logger};
			return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithTargetInterfaceGenerator(ModuleScope, interfaceToProxy) {Logger = Logger};
			return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			AssertValidType(interfaceToProxy);
			AssertValidTypes(additionalInterfacesToProxy);

			var generator = new InterfaceProxyWithoutTargetGenerator(ModuleScope, interfaceToProxy) {Logger = Logger};
			return generator.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
		}

		private void AssertValidType(Type target)
		{
			AssertValidTypeForTarget(target, target);
		}

		private void AssertValidTypeForTarget(Type type, Type target)
		{
			if (type.GetTypeInfo().IsGenericTypeDefinition)
				throw new GeneratorException(string.Format("Can not create proxy for type {0} because type {1} is an open generic type.",
					target.GetBestName(), type.GetBestName()));
			if (IsPublic(type) == false && IsAccessible(type) == false)
				throw new GeneratorException(ExceptionMessageBuilder.CreateMessageForInaccessibleType(type, target));
			foreach (var typeArgument in type.GetTypeInfo().GetGenericArguments())
				AssertValidTypeForTarget(typeArgument, target);
		}

		private void AssertValidTypes(IEnumerable<Type> targetTypes)
		{
			if (targetTypes != null)
				foreach (var t in targetTypes)
					AssertValidType(t);
		}

		private bool IsAccessible(Type target)
		{
			return IsInternal(target) && target.GetTypeInfo().Assembly.IsInternalToDynamicProxy();
		}

		private bool IsPublic(Type target)
		{
			return target.GetTypeInfo().IsPublic || target.GetTypeInfo().IsNestedPublic;
		}

		private static bool IsInternal(Type target)
		{
			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (target.GetTypeInfo().IsNestedAssembly || target.GetTypeInfo().IsNestedFamORAssem);
			var isInternalNotNested = target.GetTypeInfo().IsVisible == false && isTargetNested == false;

			return isInternalNotNested || isNestedAndInternal;
		}
	}
}