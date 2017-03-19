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
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.SubSystems.Configuration;

namespace Castle.Windsor.Windsor.Configuration.Interpreters
{
	public abstract class AbstractInterpreter : IConfigurationInterpreter
	{
		protected static readonly string ContainersNodeName = "containers";
		protected static readonly string ContainerNodeName = "container";
		protected static readonly string FacilitiesNodeName = "facilities";
		protected static readonly string FacilityNodeName = "facility";
		protected static readonly string ComponentsNodeName = "components";
		protected static readonly string ComponentNodeName = "component";
		protected static readonly string InstallersNodeName = "installers";
		protected static readonly string InstallNodeName = "install";
		private readonly Stack<IResource> resourceStack = new Stack<IResource>();

		protected AbstractInterpreter(IResource source)
		{
			if (source == null)
				throw new ArgumentNullException("source", "IResource is null");

			Source = source;

			PushResource(source);
		}

		public AbstractInterpreter(string filename) : this(new FileResource(filename))
		{
		}

		public AbstractInterpreter() : this(new ConfigResource())
		{
		}

		protected IResource CurrentResource
		{
			get
			{
				if (resourceStack.Count == 0)
					return null;

				return resourceStack.Peek();
			}
		}

		public abstract void ProcessResource(IResource resource, IConfigurationStore store, IKernel kernel);

		public IResource Source { get; }

		public string EnvironmentName { get; set; }

		protected void PushResource(IResource resource)
		{
			resourceStack.Push(resource);
		}

		protected void PopResource()
		{
			resourceStack.Pop();
		}

		protected static void AddChildContainerConfig(string name, IConfiguration childContainer, IConfigurationStore store)
		{
			AssertValidId(name);

			// TODO: Use import collection on type attribute (if it exists)

			store.AddChildContainerConfiguration(name, childContainer);
		}

		protected static void AddFacilityConfig(string id, IConfiguration facility, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type attribute (if it exists)

			store.AddFacilityConfiguration(id, facility);
		}

		protected static void AddComponentConfig(string id, IConfiguration component, IConfigurationStore store)
		{
			AssertValidId(id);

			// TODO: Use import collection on type and service attribute (if they exist)

			store.AddComponentConfiguration(id, component);
		}

		protected static void AddInstallerConfig(IConfiguration installer, IConfigurationStore store)
		{
			store.AddInstallerConfiguration(installer);
		}

		private static void AssertValidId(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				const string message = "Component or Facility was declared without a proper 'id' or 'type' attribute.";
				throw new ConfigurationProcessingException(message);
			}
		}
	}
}