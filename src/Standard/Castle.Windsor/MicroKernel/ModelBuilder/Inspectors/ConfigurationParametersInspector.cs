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
using Castle.Core.Core.Configuration;
using Castle.Windsor.Core;
using Castle.Windsor.Core.Internal;
using Castle.Windsor.MicroKernel.Util;

namespace Castle.Windsor.MicroKernel.ModelBuilder.Inspectors
{
	public class ConfigurationParametersInspector : IContributeComponentModelConstruction
	{
		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null)
				return;

			var parameters = model.Configuration.Children["parameters"];
			if (parameters == null)
				return;

			foreach (var parameter in parameters.Children)
			{
				var name = parameter.Name;
				var value = parameter.Value;

				if (value == null && parameter.Children.Count != 0)
				{
					var parameterValue = parameter.Children[0];
					model.Parameters.Add(name, parameterValue);
				}
				else
				{
					model.Parameters.Add(name, value);
				}
			}

			// Experimental code
			InspectCollections(model);
		}

		private void AddAnyServiceOverrides(ComponentModel model, IConfiguration config, ParameterModel parameter)
		{
			foreach (var item in config.Children)
			{
				if (item.Children.Count > 0)
					AddAnyServiceOverrides(model, item, parameter);

				var componentName = ReferenceExpressionUtil.ExtractComponentName(item.Value);
				if (componentName == null)
					continue;
				model.Dependencies.Add(new ComponentDependencyModel(componentName));
			}
		}

		private void InspectCollections(ComponentModel model)
		{
			foreach (var parameter in model.Parameters)
			{
				if (parameter.ConfigValue != null)
					if (IsArray(parameter) || IsList(parameter))
						AddAnyServiceOverrides(model, parameter.ConfigValue, parameter);

				if (ReferenceExpressionUtil.IsReference(parameter.Value))
					model.Dependencies.Add(new DependencyModel(parameter.Name, null, false));
			}
		}

		private bool IsArray(ParameterModel parameter)
		{
			return parameter.ConfigValue.Name.EqualsText("array");
		}

		private bool IsList(ParameterModel parameter)
		{
			return parameter.ConfigValue.Name.EqualsText("list");
		}
	}
}