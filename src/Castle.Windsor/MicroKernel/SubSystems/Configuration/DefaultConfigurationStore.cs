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

using Castle.Core.Core.Configuration;
using Castle.Core.Core.Resource;

namespace Castle.MicroKernel.SubSystems.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using Castle.MicroKernel.SubSystems.Resource;

	[Serializable]
	public class DefaultConfigurationStore : AbstractSubSystem, IConfigurationStore
	{
		private readonly IDictionary<string, IConfiguration> childContainers = new Dictionary<string, IConfiguration>();
		private readonly IDictionary<string, IConfiguration> components = new Dictionary<string, IConfiguration>();
		private readonly IDictionary<string, IConfiguration> facilities = new Dictionary<string, IConfiguration>();
		private readonly ICollection<IConfiguration> installers = new List<IConfiguration>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddChildContainerConfiguration(String key, IConfiguration config)
		{
			childContainers[key] = config;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			components[key] = config;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			facilities[key] = config;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddInstallerConfiguration(IConfiguration config)
		{
			installers.Add(config);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetChildContainerConfiguration(String key)
		{
			IConfiguration value;
			childContainers.TryGetValue(key, out value);
			return value;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetComponentConfiguration(String key)
		{
			IConfiguration value;
			components.TryGetValue(key, out value);
			return value;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetComponents()
		{
			return components.Values.ToArray();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetConfigurationForChildContainers()
		{
			return childContainers.Values.ToArray();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetFacilities()
		{
			return facilities.Values.ToArray();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetFacilityConfiguration(String key)
		{
			IConfiguration value;
			facilities.TryGetValue(key, out value);
			return value;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetInstallers()
		{
			return installers.ToArray();
		}

		public IResource GetResource(String resourceUri, IResource resource)
		{
			if (resourceUri.IndexOf(Uri.SchemeDelimiter) == -1)
			{
				return resource.CreateRelative(resourceUri);
			}

			var subSystem = (IResourceSubSystem)Kernel.GetSubSystem(SubSystemConstants.ResourceKey);

			return subSystem.CreateResource(resourceUri, resource.FileBasePath);
		}

		public override void Terminate()
		{
		}
	}
}
