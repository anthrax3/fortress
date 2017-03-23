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

using Castle.Facilities.FactorySupport;
using Castle.Windsor.Tests.Components;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Facilities.FactorySupport
{
	[TestFixture]
	public class FactorySupportProgrammaticTestCase : AbstractContainerTestCase
	{
		protected override void AfterContainerCreated()
		{
			facility = new FactorySupportFacility();
			Kernel.AddFacility(facility);
		}

		private FactorySupportFacility facility;

		private class SettingsConsumer
		{
			public SettingsConsumer(int something)
			{
				Something = something;
			}

			public int Something { get; }
		}

		[Test]
		public void FactoryResolveWithProposedFacilityPatch()
		{
			var serviceKey = "someService";
#pragma warning disable 0618 //call to obsolete method
			facility.AddFactory<ISomeService, ServiceFactory>(serviceKey, "Create");
#pragma warning restore

			var service = Kernel.Resolve(serviceKey,
				typeof(ISomeService)) as ISomeService;

			Assert.IsTrue(ServiceFactory.CreateWasCalled);
			Assert.IsInstanceOf(typeof(ServiceImplementation), service);
		}
	}
}