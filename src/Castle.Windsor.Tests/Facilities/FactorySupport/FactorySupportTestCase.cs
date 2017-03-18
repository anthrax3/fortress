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
using Castle.Core.Core.Configuration;
using Castle.Facilities.FactorySupport;
using Castle.Windsor.Core;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.ClassComponents;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Facilities.FactorySupport
{
	[TestFixture]
	public class FactorySupportTestCase : AbstractContainerTestCase
	{
		protected override void AfterContainerCreated()
		{
			Kernel.AddFacility<FactorySupportFacility>();
		}

		[Test]
		public void NullModelConfigurationBug()
		{
			Kernel.Register(Component.For<ICustomer>().Named("a").Instance(new CustomerImpl()));
		}

		[Test]
		public void Missing_dependencies_are_ignored()
		{
			Kernel.Register(Component.For<Factory>().Named("a"));

			AddComponent("stringdictComponent", typeof(StringDictionaryDependentComponent), "CreateWithStringDictionary");
			AddComponent("hashtableComponent", typeof(HashTableDependentComponent), "CreateWithHashtable");
			AddComponent("serviceComponent", typeof(ServiceDependentComponent), "CreateWithService");

			Kernel.Resolve("hashtableComponent", typeof(HashTableDependentComponent));
			Kernel.Resolve("serviceComponent", typeof(ServiceDependentComponent));
			Kernel.Resolve("stringdictComponent", typeof(StringDictionaryDependentComponent));
		}

		private ComponentModel AddComponent(string key, Type type, string factoryMethod)
		{
			var config = new MutableConfiguration(key);
			config.Attributes["factoryId"] = "a";
			config.Attributes["factoryCreate"] = factoryMethod;
			Kernel.ConfigurationStore.AddComponentConfiguration(key, config);
			Kernel.Register(Component.For(type).Named(key));
			return Kernel.GetHandler(key).ComponentModel;
		}

		public class Factory
		{
			public static MyCoolServiceWithProperties CreateCoolService(string someProperty)
			{
				return new MyCoolServiceWithProperties();
			}

			public static HashTableDependentComponent CreateWithHashtable()
			{
				return new HashTableDependentComponent(null);
			}

			public static ServiceDependentComponent CreateWithService()
			{
				return new ServiceDependentComponent(null);
			}

			public static StringDictionaryDependentComponent CreateWithStringDictionary()
			{
				return new StringDictionaryDependentComponent(null);
			}
		}

		public class MyCoolServiceWithProperties
		{
			public string SomeProperty { get; set; }
		}

		public class StringDictionaryDependentComponent
		{
			public StringDictionaryDependentComponent(Dictionary<string, object> d)
			{
			}
		}

		public class ServiceDependentComponent
		{
			public ServiceDependentComponent(ICommon d)
			{
			}
		}

		public class HashTableDependentComponent
		{
			public HashTableDependentComponent(Dictionary<object, object> d)
			{
			}
		}
	}
}
