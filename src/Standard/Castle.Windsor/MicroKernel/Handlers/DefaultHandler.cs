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
using System.Text;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.ComponentActivator;
using Castle.Windsor.MicroKernel.Context;

namespace Castle.Windsor.MicroKernel.Handlers
{
	public class DefaultHandler : AbstractHandler
	{
		public DefaultHandler(ComponentModel model) : base(model)
		{
		}

		protected ILifestyleManager LifestyleManager { get; private set; }

		public override void Dispose()
		{
			LifestyleManager.Dispose();
		}

		public override bool ReleaseCore(Burden burden)
		{
			return LifestyleManager.Release(burden.Instance);
		}

		protected void AssertNotWaitingForDependency()
		{
			if (CurrentState == HandlerState.WaitingDependency)
				throw UnresolvableHandlerException();
		}

		protected override void InitDependencies()
		{
			var activator = Kernel.CreateComponentActivator(ComponentModel);
			LifestyleManager = Kernel.CreateLifestyleManager(ComponentModel, activator);

			var awareActivator = activator as IDependencyAwareActivator;
			if (awareActivator != null && awareActivator.CanProvideRequiredDependencies(ComponentModel))
			{
				foreach (var dependency in ComponentModel.Dependencies)
					dependency.Init(ComponentModel.ParametersInternal);
				return;
			}

			base.InitDependencies();
		}

		protected override object Resolve(CreationContext context, bool instanceRequired)
		{
			Burden burden;
			return ResolveCore(context, false, instanceRequired, out burden);
		}

		protected object ResolveCore(CreationContext context, bool requiresDecommission, bool instanceRequired, out Burden burden)
		{
			if (IsBeingResolvedInContext(context))
			{
				var cache = LifestyleManager as IContextLifestyleManager;
				if (cache != null)
				{
					var instance = cache.GetContextInstance(context);
					if (instance != null)
					{
						burden = null;
						return instance;
					}
				}

				if (instanceRequired == false)
				{
					burden = null;
					return null;
				}
				var message = new StringBuilder();
				message.AppendFormat("Dependency cycle has been detected when trying to resolve component '{0}'.",
					ComponentModel.Name);
				message.AppendLine();
				message.AppendLine("The resolution tree that resulted in the cycle is the following:");
				context.BuildCycleMessageFor(this, message);

				throw new CircularDependencyException(message.ToString(), ComponentModel);
			}
			if (CanResolvePendingDependencies(context) == false)
			{
				if (instanceRequired == false)
				{
					burden = null;
					return null;
				}

				AssertNotWaitingForDependency();
			}
			try
			{
				using (var ctx = context.EnterResolutionContext(this, requiresDecommission))
				{
					var instance = LifestyleManager.Resolve(context, context.ReleasePolicy);
					burden = ctx.Burden;
					return instance;
				}
			}
			catch (NoResolvableConstructorFoundException)
			{
				throw UnresolvableHandlerException();
			}
		}

		private HandlerException UnresolvableHandlerException()
		{
			var message = new StringBuilder("Can't create component '");
			message.Append(ComponentModel.Name);
			message.AppendLine("' as it has dependencies to be satisfied.");

			var inspector = new DependencyInspector(message);
			ObtainDependencyDetails(inspector);

			return new HandlerException(inspector.Message, ComponentModel.ComponentName);
		}
	}
}