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
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Castle.MicroKernel.ModelBuilder
{
	public class DefaultComponentModelBuilder : IComponentModelBuilder
	{
		private readonly List<IContributeComponentModelConstruction> contributors = new List<IContributeComponentModelConstruction>();
		private readonly IKernel kernel;

		public DefaultComponentModelBuilder(IKernel kernel)
		{
			this.kernel = kernel;
			InitializeContributors();
		}

		public IContributeComponentModelConstruction[] Contributors
		{
			get { return contributors.ToArray(); }
		}

		public void AddContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Add(contributor);
		}

		public ComponentModel BuildModel(ComponentName name, Type[] services, Type classType, IDictionary extendedProperties)
		{
			var model = new ComponentModel(name, services, classType, extendedProperties);

            contributors.ForEach(c => c.ProcessModel(kernel, model));

			return model;
		}

		public ComponentModel BuildModel(IComponentModelDescriptor[] customContributors)
		{
			var model = new ComponentModel();

            foreach (var c in customContributors)
                c.BuildComponentModel(kernel, model);

			contributors.ForEach(c => c.ProcessModel(kernel, model));

			var metaDescriptors = default(ICollection<IMetaComponentModelDescriptor>);

		    foreach (var c in customContributors)
		    {
                c.ConfigureComponentModel(kernel, model);
                var meta = c as IMetaComponentModelDescriptor;
                if (meta != null)
                {
                    if (metaDescriptors == null)
                        metaDescriptors = model.GetMetaDescriptors(true);
                    metaDescriptors.Add(meta);
                }
            }

			return model;
		}

		public void RemoveContributor(IContributeComponentModelConstruction contributor)
		{
			contributors.Remove(contributor);
		}

		protected virtual void InitializeContributors()
		{
			var conversionManager = kernel.GetConversionManager();
			AddContributor(new GenericInspector());
			AddContributor(new ConfigurationModelInspector());
			AddContributor(new ConfigurationParametersInspector());
			AddContributor(new LifestyleModelInspector(conversionManager));
			AddContributor(new ConstructorDependenciesModelInspector());
			AddContributor(new PropertiesDependenciesModelInspector(conversionManager));
			AddContributor(new LifecycleModelInspector());
			AddContributor(new InterceptorInspector());
			AddContributor(new MixinInspector());
			AddContributor(new ComponentActivatorInspector(conversionManager));
			AddContributor(new ComponentProxyInspector(conversionManager));
		}
	}
}