// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using Castle.Windsor.Tests.ClassComponents;
using Castle.Windsor.Windsor;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Configuration2
{
	[TestFixture]
	public class ConfigurationForwardedTypesTestCase
	{
		[SetUp]
		public void SetUp()
		{
			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Configuration2/config_with_forwarded_types.xml"));
		}

		private IWindsorContainer container;

		[Test]
		public void Component_with_forwarded_types()
		{
			var first = container.Resolve<ICommon>("hasForwards");
			var second = container.Resolve<ICommon2>();
			Assert.AreSame(first, second);
		}
	}
}