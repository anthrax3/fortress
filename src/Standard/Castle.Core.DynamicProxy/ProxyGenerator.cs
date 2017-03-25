// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Castle.Core.Core.Internal;
using Castle.Core.Core.Logging;
using Castle.Core.DynamicProxy.Generators;

namespace Castle.Core.DynamicProxy
{
	public class ProxyGenerator : IProxyGenerator
	{
		private ILogger logger = NullLogger.Instance;

		public ProxyGenerator(IProxyBuilder builder)
		{
			ProxyBuilder = builder;
		}

		public ProxyGenerator() : this(new DefaultProxyBuilder())
		{
		}

		public ProxyGenerator(bool disableSignedModule) : this(new DefaultProxyBuilder(new ModuleScope(false, disableSignedModule)))
		{
		}

		public ILogger Logger
		{
			get { return logger; }
			set
			{
				logger = value;
				ProxyBuilder.Logger = value;
			}
		}

		public IProxyBuilder ProxyBuilder { get; }

		public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors)
			where TInterface : class
		{
			return
				(TInterface)
				CreateInterfaceProxyWithTarget(typeof(TInterface), target, ProxyGenerationOptions.Default, interceptors);
		}

		public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithTarget(typeof(TInterface), target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, target, ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, object target, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, null, target, options, interceptors);
		}

		public object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, object target,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTarget(interfaceToProxy, additionalInterfacesToProxy, target,
				ProxyGenerationOptions.Default, interceptors);
		}

		public virtual object CreateInterfaceProxyWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			object target,
			ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			if (interfaceToProxy == null)
				throw new ArgumentNullException("interfaceToProxy");
			if (target == null)
				throw new ArgumentNullException("target");
			if (interceptors == null)
				throw new ArgumentNullException("interceptors");

			if (!interfaceToProxy.GetTypeInfo().IsInterface)
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");

			var targetType = target.GetType();
			if (!interfaceToProxy.GetTypeInfo().IsAssignableFrom(targetType))
				throw new ArgumentException("Target does not implement interface " + interfaceToProxy.FullName, "target");

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithTarget(interfaceToProxy, additionalInterfacesToProxy, targetType,
				options);

			var arguments = GetConstructorArguments(target, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, target, ProxyGenerationOptions.Default, interceptors);
		}

		public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
			params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithTargetInterface(typeof(TInterface),
				target,
				ProxyGenerationOptions.Default,
				interceptors);
		}

		public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target,
			ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithTargetInterface(typeof(TInterface),
				target,
				options,
				interceptors);
		}

		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			object target, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy, target,
				ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy, object target,
			ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithTargetInterface(interfaceToProxy, null, target, options, interceptors);
		}

		public virtual object CreateInterfaceProxyWithTargetInterface(Type interfaceToProxy,
			Type[] additionalInterfacesToProxy,
			object target, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			//TODO: add <example> to xml comments to show how to use IChangeProxyTarget

			if (interfaceToProxy == null)
				throw new ArgumentNullException("interfaceToProxy");
			// In the case of a transparent proxy, the call to IsInstanceOfType was executed on the real object.
			if (target != null && interfaceToProxy.GetTypeInfo().IsInstanceOfType(target) == false)
				throw new ArgumentException("Target does not implement interface " + interfaceToProxy.FullName, "target");
			if (interceptors == null)
				throw new ArgumentNullException("interceptors");

			if (!interfaceToProxy.GetTypeInfo().IsInterface)
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");

			var isRemotingProxy = false;

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy,
				options);
			var arguments = GetConstructorArguments(target, interceptors, options);
			if (isRemotingProxy)
			{
				var constructors = generatedType.GetTypeInfo().GetConstructors();

				// one .ctor to rule them all
				Debug.Assert(constructors.Length == 1, "constructors.Length == 1");
				return constructors[0].Invoke(arguments.ToArray());
			}
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(IInterceptor interceptor)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithoutTarget(typeof(TInterface), interceptor);
		}

		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithoutTarget(typeof(TInterface), interceptors);
		}

		public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
			where TInterface : class
		{
			return (TInterface) CreateInterfaceProxyWithoutTarget(typeof(TInterface), Type.EmptyTypes, options, interceptors);
		}

		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, IInterceptor interceptor)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, ProxyGenerationOptions.Default,
				interceptor);
		}

		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, ProxyGenerationOptions.Default,
				interceptors);
		}

		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, additionalInterfacesToProxy,
				ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			return CreateInterfaceProxyWithoutTarget(interfaceToProxy, Type.EmptyTypes, options, interceptors);
		}

		public virtual object CreateInterfaceProxyWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			if (interfaceToProxy == null)
				throw new ArgumentNullException("interfaceToProxy");
			if (interceptors == null)
				throw new ArgumentNullException("interceptors");

			if (!interfaceToProxy.GetTypeInfo().IsInterface)
				throw new ArgumentException("Specified type is not an interface", "interfaceToProxy");

			CheckNotGenericTypeDefinition(interfaceToProxy, "interfaceToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var generatedType = CreateInterfaceProxyTypeWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options);
			var arguments = GetConstructorArguments(null, interceptors, options);
			return Activator.CreateInstance(generatedType, arguments.ToArray());
		}

		public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors)
			where TClass : class
		{
			return (TClass) CreateClassProxyWithTarget(typeof(TClass),
				Type.EmptyTypes,
				target,
				ProxyGenerationOptions.Default,
				new object[0],
				interceptors);
		}

		public TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGenerationOptions options,
			params IInterceptor[] interceptors) where TClass : class
		{
			return (TClass) CreateClassProxyWithTarget(typeof(TClass),
				Type.EmptyTypes,
				target,
				options,
				new object[0],
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				additionalInterfacesToProxy,
				target,
				ProxyGenerationOptions.Default,
				new object[0],
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
			object[] constructorArguments, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				Type.EmptyTypes,
				target,
				options,
				constructorArguments,
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, object target, object[] constructorArguments,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				Type.EmptyTypes,
				target,
				ProxyGenerationOptions.Default,
				constructorArguments,
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, object target, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				Type.EmptyTypes,
				target,
				ProxyGenerationOptions.Default,
				new object[0],
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, object target, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				Type.EmptyTypes,
				target,
				options,
				new object[0],
				interceptors);
		}

		public object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
			ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxyWithTarget(classToProxy,
				additionalInterfacesToProxy,
				target,
				options,
				new object[0],
				interceptors);
		}

		public virtual object CreateClassProxyWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, object target,
			ProxyGenerationOptions options, object[] constructorArguments,
			params IInterceptor[] interceptors)
		{
			if (classToProxy == null)
				throw new ArgumentNullException("classToProxy");
			if (options == null)
				throw new ArgumentNullException("options");
			if (!classToProxy.GetTypeInfo().IsClass)
				throw new ArgumentException("'classToProxy' must be a class", "classToProxy");

			CheckNotGenericTypeDefinition(classToProxy, "classToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var proxyType = CreateClassProxyTypeWithTarget(classToProxy, additionalInterfacesToProxy, options);

			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = BuildArgumentListForClassProxyWithTarget(target, options, interceptors);
			if (constructorArguments != null && constructorArguments.Length != 0)
				arguments.AddRange(constructorArguments);
			return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
		}

		public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
		{
			return (TClass) CreateClassProxy(typeof(TClass), ProxyGenerationOptions.Default, interceptors);
		}

		public TClass CreateClassProxy<TClass>(ProxyGenerationOptions options, params IInterceptor[] interceptors)
			where TClass : class
		{
			return (TClass) CreateClassProxy(typeof(TClass), options, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, additionalInterfacesToProxy, ProxyGenerationOptions.Default, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, object[] constructorArguments,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, options, constructorArguments, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, object[] constructorArguments, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, ProxyGenerationOptions.Default, constructorArguments, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, ProxyGenerationOptions.Default,
				null, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, ProxyGenerationOptions options, params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, null, options, interceptors);
		}

		public object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options,
			params IInterceptor[] interceptors)
		{
			return CreateClassProxy(classToProxy, additionalInterfacesToProxy, options, null, interceptors);
		}

		public virtual object CreateClassProxy(Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options,
			object[] constructorArguments, params IInterceptor[] interceptors)
		{
			if (classToProxy == null)
				throw new ArgumentNullException("classToProxy");
			if (options == null)
				throw new ArgumentNullException("options");
			if (!classToProxy.GetTypeInfo().IsClass)
				throw new ArgumentException("'classToProxy' must be a class", "classToProxy");

			CheckNotGenericTypeDefinition(classToProxy, "classToProxy");
			CheckNotGenericTypeDefinitions(additionalInterfacesToProxy, "additionalInterfacesToProxy");

			var proxyType = CreateClassProxyType(classToProxy, additionalInterfacesToProxy, options);

			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = BuildArgumentListForClassProxy(options, interceptors);
			if (constructorArguments != null && constructorArguments.Length != 0)
				arguments.AddRange(constructorArguments);
			return CreateClassProxyInstance(proxyType, arguments, classToProxy, constructorArguments);
		}

		protected List<object> GetConstructorArguments(object target, IInterceptor[] interceptors,
			ProxyGenerationOptions options)
		{
			// create constructor arguments (initialized with mixin implementations, interceptors and target type constructor arguments)
			var arguments = new List<object>(options.MixinData.Mixins) {interceptors, target};
			if (options.Selector != null)
				arguments.Add(options.Selector);
			return arguments;
		}

		protected object CreateClassProxyInstance(Type proxyType, List<object> proxyArguments, Type classToProxy,
			object[] constructorArguments)
		{
			try
			{
				return Activator.CreateInstance(proxyType, proxyArguments.ToArray());
			}
			catch (MissingMethodException)
			{
				var message = new StringBuilder();
				message.AppendFormat("Can not instantiate proxy of class: {0}.", classToProxy.FullName);
				message.AppendLine();
				if (constructorArguments == null || constructorArguments.Length == 0)
				{
					message.Append("Could not find a parameterless constructor.");
				}
				else
				{
					message.AppendLine("Could not find a constructor that would match given arguments:");
					foreach (var argument in constructorArguments)
					{
						var argumentText = argument == null ? "<null>" : argument.GetType().ToString();
						message.AppendLine(argumentText);
					}
				}

				throw new InvalidProxyConstructorArgumentsException(message.ToString(), proxyType, classToProxy);
			}
		}

		protected void CheckNotGenericTypeDefinition(Type type, string argumentName)
		{
			if (type != null && type.GetTypeInfo().IsGenericTypeDefinition)
				throw new GeneratorException(string.Format("Can not create proxy for type {0} because it is an open generic type.",
					type.GetBestName()));
		}

		protected void CheckNotGenericTypeDefinitions(IEnumerable<Type> types, string argumentName)
		{
			if (types == null)
				return;
			foreach (var t in types)
				CheckNotGenericTypeDefinition(t, argumentName);
		}

		protected List<object> BuildArgumentListForClassProxyWithTarget(object target, ProxyGenerationOptions options,
			IInterceptor[] interceptors)
		{
			var arguments = new List<object>();
			arguments.Add(target);
			arguments.AddRange(options.MixinData.Mixins);
			arguments.Add(interceptors);
			if (options.Selector != null)
				arguments.Add(options.Selector);
			return arguments;
		}

		protected List<object> BuildArgumentListForClassProxy(ProxyGenerationOptions options, IInterceptor[] interceptors)
		{
			var arguments = new List<object>(options.MixinData.Mixins) {interceptors};
			if (options.Selector != null)
				arguments.Add(options.Selector);
			return arguments;
		}

		protected Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxyType(classToProxy, additionalInterfacesToProxy, options);
		}

		protected Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			Type targetType,
			ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTarget(interfaceToProxy, additionalInterfacesToProxy, targetType,
				options);
		}

		protected Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(interfaceToProxy, additionalInterfacesToProxy,
				options);
		}

		protected Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(interfaceToProxy, additionalInterfacesToProxy, options);
		}

		protected Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy,
			ProxyGenerationOptions options)
		{
			// create proxy
			return ProxyBuilder.CreateClassProxyTypeWithTarget(classToProxy, additionalInterfacesToProxy, options);
		}
	}
}