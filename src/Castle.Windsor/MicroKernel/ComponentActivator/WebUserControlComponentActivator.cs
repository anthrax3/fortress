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



namespace Castle.MicroKernel.ComponentActivator
{
	using System;
	using System.Web;
	using System.Web.UI;

	using Castle.Core;
	using Castle.MicroKernel.Context;

	[Serializable]
	public class WebUserControlComponentActivator : DefaultComponentActivator
	{
		public WebUserControlComponentActivator(ComponentModel model, IKernelInternal kernel,
		                                        ComponentInstanceDelegate onCreation,
		                                        ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
		{
			object instance = null;

			var implType = Model.Implementation;

			var createProxy = Model.HasInterceptors;
			var createInstance = true;

			if (createProxy)
			{
				createInstance = Kernel.ProxyFactory.RequiresTargetInstance(Kernel, Model);
			}

			if (createInstance)
			{
				try
				{
					var currentContext = HttpContext.Current;
					if (currentContext == null)
					{
						throw new InvalidOperationException(
							"System.Web.HttpContext.Current is null.  WebUserControlComponentActivator can only be used in an ASP.Net environment.");
					}

					var currentPage = currentContext.Handler as Page;
					if (currentPage == null)
					{
						throw new InvalidOperationException("System.Web.HttpContext.Current.Handler is not of type System.Web.UI.Page");
					}

					var virtualPath = Model.Configuration.Attributes["VirtualPath"];
					if (!string.IsNullOrEmpty(virtualPath))
					{
						instance = currentPage.LoadControl(virtualPath);
					}
					else
					{
						instance = currentPage.LoadControl(implType, arguments);
					}
				}
				catch (Exception ex)
				{
					throw new ComponentActivatorException(
						"WebUserControlComponentActivator: could not instantiate " + Model.Implementation.FullName, ex, Model);
				}
			}

			if (createProxy)
			{
				try
				{
					instance = Kernel.ProxyFactory.Create(Kernel, instance, Model, context, arguments);
				}
				catch (Exception ex)
				{
					throw new ComponentActivatorException("ComponentActivator: could not proxy " + Model.Implementation.FullName, ex, Model);
				}
			}

			return instance;
		}
	}
}

