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

using System;
using System.IO;
using Castle.Facilities.Logging;
using Castle.Windsor.MicroKernel.Registration;
using Castle.Windsor.Tests.LoggingFacility.Classes;
using Castle.Windsor.Windsor;
using NUnit.Framework;

namespace Castle.Windsor.Tests.LoggingFacility
{
	[TestFixture]
	public class ConsoleFacilityTestCase : BaseTest
	{
		[SetUp]
		public void Setup()
		{
			container = CreateConfiguredContainer(LoggerImplementation.Console);

			outWriter.GetStringBuilder().Length = 0;
			errorWriter.GetStringBuilder().Length = 0;

			Console.SetOut(outWriter);
			Console.SetError(errorWriter);
		}

		[TearDown]
		public void Teardown()
		{
			if (container != null)
				container.Dispose();
		}

		private IWindsorContainer container;
		private readonly StringWriter outWriter = new StringWriter();
		private readonly StringWriter errorWriter = new StringWriter();

		[Test]
		public void SimpleTest()
		{
			container.Register(Component.For(typeof(SimpleLoggingComponent)).Named("component"));
			var test = container.Resolve<SimpleLoggingComponent>("component");

			var expectedLogOutput = string.Format("[Info] '{0}' Hello world" + Environment.NewLine, typeof(SimpleLoggingComponent).FullName);
			var actualLogOutput = "";

			test.DoSomething();

			actualLogOutput = outWriter.GetStringBuilder().ToString();
			Assert.AreEqual(expectedLogOutput, actualLogOutput);
		}
	}
}