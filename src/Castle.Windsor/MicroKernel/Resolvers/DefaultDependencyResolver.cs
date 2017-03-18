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
using System.Linq;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.Context;
using Castle.Windsor.MicroKernel.Handlers;
using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.MicroKernel.Resolvers
{
	[Serializable]
	public class DefaultDependencyResolver : IDependencyResolver
	{
		private readonly IList<ISubDependencyResolver> subResolvers = new List<ISubDependencyResolver>();
		private ITypeConverter converter;
		private DependencyDelegate dependencyResolvingDelegate;
		private IKernelInternal kernel;

		public void AddSubResolver(ISubDependencyResolver subResolver)
		{
			if (subResolver == null)
			{
				throw new ArgumentNullException("subResolver");
			}

			subResolvers.Add(subResolver);
		}

		public void Initialize(IKernelInternal kernel, DependencyDelegate dependencyDelegate)
		{
			this.kernel = kernel;
			converter = kernel.GetConversionManager();
			dependencyResolvingDelegate = dependencyDelegate;
		}

		public void RemoveSubResolver(ISubDependencyResolver subResolver)
		{
			subResolvers.Remove(subResolver);
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			// 1 - check for the dependency on CreationContext, if present
			if (CanResolveFromContext(context, contextHandlerResolver, model, dependency))
			{
				return true;
			}

			// 2 - check with the model's handler, if not the same as the parent resolver
			if (CanResolveFromHandler(context, contextHandlerResolver, model, dependency))
			{
				return true;
			}

			// 3 - check within parent resolver, if present
			if (CanResolveFromContextHandlerResolver(context, contextHandlerResolver, model, dependency))
			{
				return true;
			}

			// 4 - check within subresolvers
			if (CanResolveFromSubResolvers(context, contextHandlerResolver, model, dependency))
			{
				return true;
			}

			// 5 - normal flow, checking against the kernel
			return CanResolveFromKernel(context, model, dependency);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			var value = ResolveCore(context, contextHandlerResolver, model, dependency);
			if (value == null)
			{
				if (dependency.HasDefaultValue)
				{
					value = dependency.DefaultValue;
				}
				else if (dependency.IsOptional == false)
				{
					var message = String.Format(
						"Could not resolve non-optional dependency for '{0}' ({1}). Parameter '{2}' type '{3}'",
						model.Name,
						model.Implementation != null ? model.Implementation.FullName : "-unknown-",
						dependency.DependencyKey,
						dependency.TargetType.FullName);

					throw new DependencyResolverException(message);
				}
			}

			dependencyResolvingDelegate(model, dependency, value);
			return value;
		}

		protected virtual bool CanResolveFromKernel(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			if (dependency.ReferencedComponentName != null)
			{
				// User wants to override
				return HasComponentInValidState(dependency.ReferencedComponentName, dependency, context);
			}
			if (dependency.Parameter != null)
			{
				return true;
			}
			if (typeof(IKernel).IsAssignableFrom(dependency.TargetItemType))
			{
				return true;
			}

			if (dependency.TargetItemType.IsPrimitiveType())
			{
				return false;
			}

			return HasAnyComponentInValidState(dependency.TargetItemType, dependency, context);
		}

		protected virtual CreationContext RebuildContextForParameter(CreationContext current, Type parameterType)
		{
			if (parameterType.ContainsGenericParameters)
			{
				return current;
			}

			return new CreationContext(parameterType, current, false);
		}

		protected virtual object ResolveFromKernel(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			if (dependency.ReferencedComponentName != null)
			{
				return ResolveFromKernelByName(context, model, dependency);
			}
			if (dependency.Parameter != null)
			{
				return ResolveFromParameter(context, model, dependency);
			}
			if (typeof(IKernel).IsAssignableFrom(dependency.TargetItemType))
			{
				return kernel;
			}
			if (dependency.TargetItemType.IsPrimitiveType())
			{
				// we can shortcircuit it here, since we know we won't find any components for value type service as those are not legal.
				return null;
			}

			return ResolveFromKernelByType(context, model, dependency);
		}

		private bool CanResolveFromContext(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			DependencyModel dependency)
		{
			return context != null && context.CanResolve(context, contextHandlerResolver, model, dependency);
		}

		private bool CanResolveFromContextHandlerResolver(CreationContext context, ISubDependencyResolver contextHandlerResolver,
			ComponentModel model, DependencyModel dependency)
		{
			return contextHandlerResolver != null && contextHandlerResolver.CanResolve(context, contextHandlerResolver, model, dependency);
		}

		private bool CanResolveFromHandler(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			DependencyModel dependency)
		{
			var handler = kernel.GetHandler(model.Name);
			var b = handler != null && handler != contextHandlerResolver && handler.CanResolve(context, contextHandlerResolver, model, dependency);
			return b;
		}

		private bool CanResolveFromSubResolvers(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
			DependencyModel dependency)
		{
			return subResolvers.Count > 0 && subResolvers.Any(s => s.CanResolve(context, contextHandlerResolver, model, dependency));
		}

		private bool HasAnyComponentInValidState(Type service, DependencyModel dependency, CreationContext context)
		{
			IHandler firstHandler;
			if (context != null && context.IsResolving)
			{
				firstHandler = kernel.LoadHandlerByType(dependency.DependencyKey, service, context.AdditionalArguments);
			}
			else
			{
				firstHandler = kernel.GetHandler(service);
			}
			if (firstHandler == null)
			{
				return false;
			}
			if (context == null || firstHandler.IsBeingResolvedInContext(context) == false)
			{
				if (IsHandlerInValidState(firstHandler))
				{
					return true;
				}
			}

			var handlers = kernel.GetHandlers(service);
			return handlers.Where(handler => handler.IsBeingResolvedInContext(context) == false)
				.Any(IsHandlerInValidState);
		}

		private bool HasComponentInValidState(string key, DependencyModel dependency, CreationContext context)
		{
			IHandler handler;
			if (context != null && context.IsResolving)
			{
				handler = kernel.LoadHandlerByName(key, dependency.TargetItemType, context.AdditionalArguments);
			}
			else
			{
				handler = kernel.GetHandler(key);
			}
			return IsHandlerInValidState(handler) && handler.IsBeingResolvedInContext(context) == false;
		}

		private object ResolveCore(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			// 1 - check for the dependency on CreationContext, if present
			if (CanResolveFromContext(context, contextHandlerResolver, model, dependency))
			{
				return context.Resolve(context, contextHandlerResolver, model, dependency);
			}

			// 2 - check with the model's handler, if not the same as the parent resolver
			var handler = kernel.GetHandler(model.Name);
			if (handler != contextHandlerResolver && handler.CanResolve(context, contextHandlerResolver, model, dependency))
			{
				return handler.Resolve(context, contextHandlerResolver, model, dependency);
			}

			// 3 - check within parent resolver, if present
			if (CanResolveFromContextHandlerResolver(context, contextHandlerResolver, model, dependency))
			{
				return contextHandlerResolver.Resolve(context, contextHandlerResolver, model, dependency);
			}

			// 4 - check within subresolvers
			if (subResolvers.Count > 0)
			{
				for (var index = 0; index < subResolvers.Count; index++)
				{
					var subResolver = subResolvers[index];
					if (subResolver.CanResolve(context, contextHandlerResolver, model, dependency))
					{
						return subResolver.Resolve(context, contextHandlerResolver, model, dependency);
					}
				}
			}

			// 5 - normal flow, checking against the kernel
			return ResolveFromKernel(context, model, dependency);
		}

		private object ResolveFromKernelByName(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			var handler = kernel.LoadHandlerByName(dependency.ReferencedComponentName, dependency.TargetItemType, context.AdditionalArguments);

			// never (famous last words) this should really happen as we're the good guys and we call CanResolve before trying to resolve but let's be safe.
			if (handler == null)
			{
				throw new DependencyResolverException(
					string.Format(
						"Missing dependency.{2}Component {0} has a dependency on component {1}, which was not registered.{2}Make sure the dependency is correctly registered in the container as a service.",
						model.Name,
						dependency.ReferencedComponentName,
						Environment.NewLine));
			}

			var contextRebuilt = RebuildContextForParameter(context, dependency.TargetItemType);

			return handler.Resolve(contextRebuilt);
		}

		private object ResolveFromKernelByType(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			IHandler handler;
			try
			{
				handler = TryGetHandlerFromKernel(dependency, context);
			}
			catch (HandlerException exception)
			{
				if (dependency.HasDefaultValue)
				{
					return dependency.DefaultValue;
				}
				throw new DependencyResolverException(
					string.Format(
						"Missing dependency.{2}Component {0} has a dependency on {1}, which could not be resolved.{2}Make sure the dependency is correctly registered in the container as a service, or provided as inline argument.",
						model.Name,
						dependency.TargetItemType,
						Environment.NewLine),
					exception);
			}

			if (handler == null)
			{
				if (dependency.HasDefaultValue)
				{
					return dependency.DefaultValue;
				}
				throw new DependencyResolverException(
					string.Format(
						"Cycle detected in configuration.{2}Component {0} has a dependency on {1}, but it doesn't provide an override.{2}You must provide an override if a component has a dependency on a service that it - itself - provides.",
						model.Name,
						dependency.TargetItemType,
						Environment.NewLine));
			}
			context = RebuildContextForParameter(context, dependency.TargetItemType);

			return handler.Resolve(context);
		}

		private object ResolveFromParameter(CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			converter.Context.Push(model, context);
			try
			{
				if (dependency.Parameter.Value != null || dependency.Parameter.ConfigValue == null)
				{
					return converter.PerformConversion(dependency.Parameter.Value, dependency.TargetItemType);
				}
				else
				{
					return converter.PerformConversion(dependency.Parameter.ConfigValue, dependency.TargetItemType);
				}
			}
			catch (ConverterException e)
			{
				throw new DependencyResolverException(
					string.Format("Could not convert parameter '{0}' to type '{1}'.", dependency.Parameter.Name, dependency.TargetItemType.Name), e);
			}
			finally
			{
				converter.Context.Pop();
			}
		}

		private IHandler TryGetHandlerFromKernel(DependencyModel dependency, CreationContext context)
		{
			// we are doing it in two stages because it is likely to be faster to a lookup
			// by key than a linear search
			var handler = kernel.LoadHandlerByType(dependency.DependencyKey, dependency.TargetItemType, context.AdditionalArguments);
			if (handler == null)
			{
				throw new HandlerException(string.Format("Handler for {0} was not found.", dependency.TargetItemType), null);
			}
			if (handler.IsBeingResolvedInContext(context) == false)
			{
				return handler;
			}

			// make a best effort to find another one that fit

			var handlers = kernel.GetHandlers(dependency.TargetItemType);
			foreach (var maybeCorrectHandler in handlers)
			{
				if (maybeCorrectHandler.IsBeingResolvedInContext(context) == false)
				{
					handler = maybeCorrectHandler;
					break;
				}
			}
			return handler;
		}

		private static bool IsHandlerInValidState(IHandler handler)
		{
			if (handler == null)
			{
				return false;
			}

			return handler.CurrentState == HandlerState.Valid;
		}
	}
}
