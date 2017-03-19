// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.Facilities;
using Castle.Windsor.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.Facilities.Startable
{
	public partial class StartableFacility : AbstractFacility
	{
		private ITypeConverter converter;
		private StartFlag flag;

		public void DeferredStart()
		{
			DeferredStart(new DeferredStartFlag());
		}

		public void DeferredStart(StartFlag flag)
		{
			this.flag = flag;
		}

		public void DeferredTryStart()
		{
			DeferredStart(new DeferredTryStartFlag());
		}

		protected override void Init()
		{
			converter = Kernel.GetConversionManager();
			Kernel.ComponentModelBuilder.AddContributor(new StartableContributor(converter));

			InitFlag(flag ?? new LegacyStartFlag(), new StartableEvents(Kernel));
		}

		private void InitFlag(IStartFlagInternal startFlag, StartableEvents events)
		{
			startFlag.Init(events);
		}

		public static bool IsStartable(IHandler handler)
		{
			var startable = handler.ComponentModel.ExtendedProperties["startable"];
			var isStartable = (bool?) startable;
			return isStartable.GetValueOrDefault();
		}
	}
}