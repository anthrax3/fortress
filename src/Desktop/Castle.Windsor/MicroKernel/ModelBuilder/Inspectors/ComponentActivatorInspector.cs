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
using System.Linq;
using Castle.Core.Core.Internal;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Inspectors
{
	[Serializable]
	public class ComponentActivatorInspector : IContributeComponentModelConstruction
	{
		private readonly IConversionManager converter;

		public ComponentActivatorInspector(IConversionManager converter)
		{
			this.converter = converter;
		}

		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (!ReadComponentActivatorFromConfiguration(model))
				ReadComponentActivatorFromType(model);
		}

		protected virtual bool ReadComponentActivatorFromConfiguration(ComponentModel model)
		{
			if (model.Configuration != null)
			{
				var componentActivatorType = model.Configuration.Attributes["componentActivatorType"];
				if (componentActivatorType == null)
					return false;

				var customComponentActivator = converter.PerformConversion<Type>(componentActivatorType);
				ValidateComponentActivator(customComponentActivator);

				model.CustomComponentActivator = customComponentActivator;
				return true;
			}

			return false;
		}

		protected virtual void ReadComponentActivatorFromType(ComponentModel model)
		{
			var attributes = AttributesUtil.GetAttributes<ComponentActivatorAttribute>(model.Implementation).ToArray();
			if (attributes.Length != 0)
			{
				var attribute = attributes[0];
				ValidateComponentActivator(attribute.ComponentActivatorType);

				model.CustomComponentActivator = attribute.ComponentActivatorType;
			}
		}

		protected virtual void ValidateComponentActivator(Type customComponentActivator)
		{
			if (customComponentActivator.Is<IComponentActivator>() == false)
			{
				var message =
					string.Format(
						"The Type '{0}' specified in the componentActivatorType attribute must implement {1}",
						customComponentActivator.FullName, typeof(IComponentActivator).FullName);
				throw new InvalidOperationException(message);
			}
		}
	}
}