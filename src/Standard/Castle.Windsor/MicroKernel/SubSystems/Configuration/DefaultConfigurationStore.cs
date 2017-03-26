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
using Castle.Core.Core.Configuration;
using Castle.Core.Core.Resource;
using Castle.Windsor.MicroKernel.SubSystems.Resource;

namespace Castle.Windsor.MicroKernel.SubSystems.Configuration
{
	public class DefaultConfigurationStore : AbstractSubSystem, IConfigurationStore
	{
	    private const string schemeDelimiter = "://";
	    private static object synchronise = new object();
		private readonly IDictionary<string, IConfiguration> childContainers = new Dictionary<string, IConfiguration>();
		private readonly IDictionary<string, IConfiguration> components = new Dictionary<string, IConfiguration>();
		private readonly IDictionary<string, IConfiguration> facilities = new Dictionary<string, IConfiguration>();
		private readonly ICollection<IConfiguration> installers = new List<IConfiguration>();

		public void AddChildContainerConfiguration(string key, IConfiguration config)
		{
            lock(synchronise)
			    childContainers[key] = config;
		}

		public void AddComponentConfiguration(string key, IConfiguration config)
		{
            lock (synchronise)
                components[key] = config;
		}

		public void AddFacilityConfiguration(string key, IConfiguration config)
		{
            lock (synchronise)
                facilities[key] = config;
		}

		public void AddInstallerConfiguration(IConfiguration config)
		{
            lock (synchronise)
                installers.Add(config);
		}

		public IConfiguration GetChildContainerConfiguration(string key)
		{
		    lock (synchronise)
		    {
		        IConfiguration value;
		        childContainers.TryGetValue(key, out value);
		        return value;
		    }
		}

		public IConfiguration GetComponentConfiguration(string key)
		{
		    lock (synchronise)
		    {
		        IConfiguration value;
		        components.TryGetValue(key, out value);
		        return value;
		    }
		}

		public IConfiguration[] GetComponents()
		{
		    lock (synchronise)
                return components.Values.ToArray();
		}

		public IConfiguration[] GetConfigurationForChildContainers()
		{
		    lock (synchronise)
                return childContainers.Values.ToArray();
		}

		public IConfiguration[] GetFacilities()
		{
		    lock (synchronise)
                return facilities.Values.ToArray();
		}

		public IConfiguration GetFacilityConfiguration(string key)
		{
		    lock (synchronise)
		    {
		        IConfiguration value;
		        facilities.TryGetValue(key, out value);
		        return value;
		    }
		}

		public IConfiguration[] GetInstallers()
		{
		    lock (synchronise)
                return installers.ToArray();
		}

		public IResource GetResource(string resourceUri, IResource resource)
		{
		    lock (synchronise)
		    {
		        if (resourceUri.IndexOf(schemeDelimiter) == -1)
		            return resource.CreateRelative(resourceUri);

		        var subSystem = (IResourceSubSystem) Kernel.GetSubSystem(SubSystemConstants.ResourceKey);

		        return subSystem.CreateResource(resourceUri, resource.FileBasePath);
		    }
		}

		public override void Terminate()
		{
		}
	}
}