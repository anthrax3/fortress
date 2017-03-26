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
using System.Linq;
using System.Reflection;
using Castle.Core.DynamicProxy;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Interceptor;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Context;
using Castle.Windsor.MicroKernel.Proxy;

namespace Castle.Windsor.Windsor.Proxy
{
	public class DefaultProxyFactory : AbstractProxyFactory
	{
		protected ProxyGenerator generator;

		public DefaultProxyFactory() : this(new ProxyGenerator())
		{
		}

		public DefaultProxyFactory(bool disableSignedModule)
			: this(new ProxyGenerator(disableSignedModule))
		{
		}

		public DefaultProxyFactory(ProxyGenerator generator)
		{
			this.generator = generator;
		}

		public void OnDeserialization(object sender)
		{
			generator = new ProxyGenerator();
		}

		public override object Create(IProxyFactoryExtension customFactory, IKernel kernel, ComponentModel model, CreationContext context,
			params object[] constructorArguments)
		{
			var interceptors = ObtainInterceptors(kernel, model, context);
			var proxyOptions = model.ObtainProxyOptions();
			var proxyGenOptions = CreateProxyGenerationOptionsFrom(proxyOptions, kernel, context, model);

			CustomizeOptions(proxyGenOptions, kernel, model, constructorArguments);
			var builder = generator.ProxyBuilder;
			var proxy = customFactory.Generate(builder, proxyGenOptions, interceptors, model, context);

			CustomizeProxy(proxy, proxyGenOptions, kernel, model);
			ReleaseHook(proxyGenOptions, kernel);
			return proxy;
		}

		private void ReleaseHook(ProxyGenerationOptions proxyGenOptions, IKernel kernel)
		{
			if (proxyGenOptions.Hook == null)
				return;
			kernel.ReleaseComponent(proxyGenOptions.Hook);
		}

		public override object Create(IKernel kernel, object target, ComponentModel model, CreationContext context, params object[] constructorArguments)
		{
			object proxy;

			var interceptors = ObtainInterceptors(kernel, model, context);
			var proxyOptions = model.ObtainProxyOptions();
			var proxyGenOptions = CreateProxyGenerationOptionsFrom(proxyOptions, kernel, context, model);

			CustomizeOptions(proxyGenOptions, kernel, model, constructorArguments);

			var interfaces = proxyOptions.AdditionalInterfaces;
			if (model.HasClassServices == false)
			{
				var firstService = model.Services.First();
				var additionalInterfaces = model.Services.Skip(1).Concat(interfaces).ToArray();
				if (proxyOptions.OmitTarget)
					proxy = generator.CreateInterfaceProxyWithoutTarget(firstService, additionalInterfaces, proxyGenOptions, interceptors);
				else if (proxyOptions.AllowChangeTarget)
					proxy = generator.CreateInterfaceProxyWithTargetInterface(firstService, additionalInterfaces, target, proxyGenOptions, interceptors);
				else
					proxy = generator.CreateInterfaceProxyWithTarget(firstService, additionalInterfaces, target, proxyGenOptions, interceptors);
			}
			else
			{
				Type classToProxy;
				if (model.Implementation != null && model.Implementation != typeof(LateBoundComponent))
					classToProxy = model.Implementation;
				else
					classToProxy = model.Services.First();
				var additionalInterfaces = model.Services
					.SkipWhile(s => s.GetTypeInfo().IsClass)
					.Concat(interfaces)
					.ToArray();
				proxy = generator.CreateClassProxy(classToProxy, additionalInterfaces, proxyGenOptions, constructorArguments, interceptors);
			}

			CustomizeProxy(proxy, proxyGenOptions, kernel, model);
			ReleaseHook(proxyGenOptions, kernel);
			return proxy;
		}

		protected static ProxyGenerationOptions CreateProxyGenerationOptionsFrom(ProxyOptions proxyOptions, IKernel kernel, CreationContext context, ComponentModel model)
		{
			var proxyGenOptions = new ProxyGenerationOptions();
			if (proxyOptions.Hook != null)
			{
				var hook = proxyOptions.Hook.Resolve(kernel, context);
				if (hook != null && hook is IOnBehalfAware)
					((IOnBehalfAware) hook).SetInterceptedComponentModel(model);
				proxyGenOptions.Hook = hook;
			}

			if (proxyOptions.Selector != null)
			{
				var selector = proxyOptions.Selector.Resolve(kernel, context);
				if (selector != null && selector is IOnBehalfAware)
					((IOnBehalfAware) selector).SetInterceptedComponentModel(model);
				proxyGenOptions.Selector = selector;
			}

			foreach (var mixInReference in proxyOptions.MixIns)
			{
				var mixIn = mixInReference.Resolve(kernel, context);
				proxyGenOptions.AddMixinInstance(mixIn);
			}

			return proxyGenOptions;
		}

		protected virtual void CustomizeProxy(object proxy, ProxyGenerationOptions options, IKernel kernel, ComponentModel model)
		{
		}

		protected virtual void CustomizeOptions(ProxyGenerationOptions options, IKernel kernel, ComponentModel model, object[] arguments)
		{
		}

		public override bool RequiresTargetInstance(IKernel kernel, ComponentModel model)
		{
			var proxyOptions = model.ObtainProxyOptions();

			return model.HasClassServices == false &&
			       proxyOptions.OmitTarget == false;
		}
	}
}