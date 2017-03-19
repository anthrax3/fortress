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
using Castle.Core.Core.Resource;

namespace Castle.Windsor.MicroKernel.SubSystems.Configuration
{
	public interface IConfigurationStore : ISubSystem
	{
		void AddChildContainerConfiguration(String name, IConfiguration config);

		void AddComponentConfiguration(String key, IConfiguration config);

		void AddFacilityConfiguration(String key, IConfiguration config);

		void AddInstallerConfiguration(IConfiguration config);

		IConfiguration GetChildContainerConfiguration(String key);

		IConfiguration GetComponentConfiguration(String key);

		IConfiguration[] GetComponents();

		IConfiguration[] GetConfigurationForChildContainers();

		IConfiguration[] GetFacilities();

		IConfiguration GetFacilityConfiguration(String key);

		IConfiguration[] GetInstallers();

		IResource GetResource(String resourceUri, IResource resource);
	}
}
