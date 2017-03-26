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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Core.Internal;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.Proxy;
using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Inspectors
{
	public class ComponentProxyInspector : IContributeComponentModelConstruction
	{
		private readonly IConversionManager converter;

		public ComponentProxyInspector(IConversionManager converter)
		{
			this.converter = converter;
		}

		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			ReadProxyBehavior(kernel, model);
		}

		protected virtual ComponentProxyBehaviorAttribute ReadProxyBehaviorFromType(Type implementation)
		{
			return AttributesUtil.GetAttributes<ComponentProxyBehaviorAttribute>(implementation).FirstOrDefault();
		}

		protected virtual void ReadProxyBehavior(IKernel kernel, ComponentModel model)
		{
			var proxyBehaviorAttribute = ReadProxyBehaviorFromType(model.Implementation);
			if (proxyBehaviorAttribute == null)
				proxyBehaviorAttribute = new ComponentProxyBehaviorAttribute();

			ReadProxyBehaviorFromConfig(model, proxyBehaviorAttribute);

			ApplyProxyBehavior(proxyBehaviorAttribute, model);
		}

		private void ReadProxyBehaviorFromConfig(ComponentModel model, ComponentProxyBehaviorAttribute behavior)
		{
			if (model.Configuration == null)
				return;

			var interfaces = model.Configuration.Children["additionalInterfaces"];
			if (interfaces == null)
				return;
			var list = new List<Type>(behavior.AdditionalInterfaces);
			foreach (var node in interfaces.Children)
			{
				var interfaceTypeName = node.Attributes["interface"];
				var @interface = converter.PerformConversion<Type>(interfaceTypeName);
				list.Add(@interface);
			}
			behavior.AdditionalInterfaces = list.ToArray();
		}

		private static void ApplyProxyBehavior(ComponentProxyBehaviorAttribute behavior, ComponentModel model)
		{
			var options = model.ObtainProxyOptions();

			options.AddAdditionalInterfaces(behavior.AdditionalInterfaces);
			if (model.Implementation.GetTypeInfo().IsInterface)
				options.OmitTarget = true;
		}

		private static void EnsureComponentRegisteredWithInterface(ComponentModel model)
		{
			if (model.HasClassServices)
			{
				var message = string.Format("The class {0} requested a single interface proxy, " +
				                            "however the service {1} does not represent an interface",
					model.Implementation.FullName, model.Services.First().FullName);

				throw new ComponentRegistrationException(message);
			}
		}
	}
}