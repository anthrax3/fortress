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
using Castle.Core.Configuration;

namespace Castle.MicroKernel.Facilities
{
	public abstract class AbstractFacility : IFacility, IDisposable
	{
		protected IConfiguration FacilityConfig { get; private set; }

		protected IKernel Kernel { get; private set; }

		void IDisposable.Dispose()
		{
			Dispose();
		}

		void IFacility.Init(IKernel kernel, IConfiguration facilityConfig)
		{
			Kernel = kernel;
			FacilityConfig = facilityConfig;

			Init();
		}

		void IFacility.Terminate()
		{
			Dispose();

			Kernel = null;
		}

		protected abstract void Init();

		protected virtual void Dispose()
		{
		}
	}
}