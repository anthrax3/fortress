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


// we do not support xml config on SL

using System.Linq;
using Castle.Core.Core.Resource;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel;
using Castle.Windsor.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Tests.ClassComponents;
using Castle.Windsor.Tests.Components;
using Castle.Windsor.Tests.XmlFiles;
using Castle.Windsor.Windsor;
using Castle.Windsor.Windsor.Configuration.Interpreters;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Config
{
	[TestFixture]
	public class ConfigXmlInterpreterTestCase
	{
		[Test]
		public void ComponentIdGetsLoadedFromTheParsedConfiguration()
		{
			var store = new DefaultConfigurationStore();
			var interpreter = new XmlInterpreter(Xml.Embedded("sample_config_with_spaces.xml"));
			IKernel kernel = new DefaultKernel();
			interpreter.ProcessResource(interpreter.Source, store, kernel);

			var container = new WindsorContainer(store);

			var handler = container.Kernel.GetHandler(typeof(ICalcService));
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void CorrectConfigurationMapping()
		{
			var store = new DefaultConfigurationStore();
			var interpreter = new XmlInterpreter(Xml.Embedded("sample_config.xml"));
			IKernel kernel = new DefaultKernel();
			interpreter.ProcessResource(interpreter.Source, store, kernel);

			var container = new WindsorContainer(store);
			var facility = container.Kernel.GetFacilities().OfType<HiperFacility>().Single();
			Assert.IsTrue(facility.Initialized);
		}

		[Test]
		public void MissingManifestResourceConfiguration()
		{
			Assert.Throws<ConfigurationProcessingException>(() =>
			{
				var store = new DefaultConfigurationStore();
				var source = new AssemblyResource("assembly://Castle.Windsor.Tests/missing_config.xml");
				IKernel kernel = new DefaultKernel();
				new XmlInterpreter(source).ProcessResource(source, store, kernel);
			});
		}
	}
}