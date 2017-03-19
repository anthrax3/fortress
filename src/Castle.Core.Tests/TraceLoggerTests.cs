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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Castle.Core.Core.Logging;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class TraceLoggerTests
	{
		[SetUp]
		public void Initialize()
		{
			Listener.ClearMessages();
		}

		[TearDown]
		public void Cleanup()
		{
			Listener.ClearMessages();
		}

		public class Listener : TraceListener
		{
			private static Dictionary<string, StringBuilder> traces = new Dictionary<string, StringBuilder>();
			private readonly string traceName;

			public Listener()
			{
			}

			public Listener(string initializationData)
			{
				traceName = initializationData;
			}

			private StringBuilder GetStringBuilder()
			{
				lock (traces)
				{
					if (!traces.ContainsKey(traceName))
						traces.Add(traceName, new StringBuilder());

					return traces[traceName];
				}
			}

			public override void Write(string message)
			{
				GetStringBuilder().Append(message);
			}

			public override void WriteLine(string message)
			{
				GetStringBuilder().AppendLine(message);
			}

			public static void AssertContains(string traceName, string expected)
			{
				Assert.IsTrue(traces.ContainsKey(traceName), "Trace named {0} not found", traceName);
				Assert.IsTrue(traces[traceName].ToString().Contains(expected), string.Format("Trace text expected to contain '{0}'", expected));
			}

			public static void ClearMessages()
			{
				traces = new Dictionary<string, StringBuilder>();
			}
		}

		[Test]
		public void FallUpToDefaultSource()
		{
			var factory = new TraceLoggerFactory();
			var logger = factory.Create("System.Xml.XmlDocument", LoggerLevel.Debug);
			logger.Info("Logging to non-configured namespace namespace");

			Listener.AssertContains("defaultrule", "System.Xml.XmlDocument");
			Listener.AssertContains("defaultrule", "Logging to non-configured namespace namespace");
		}

		[Test]
		public void TracingErrorInformation()
		{
			var factory = new TraceLoggerFactory();
			var logger = factory.Create(typeof(TraceLoggerTests), LoggerLevel.Debug);
			try
			{
				try
				{
					var fakearg = "Thisisavalue";
					throw new ArgumentOutOfRangeException("fakearg", fakearg, "Thisisamessage");
				}
				catch (Exception ex)
				{
					throw new Exception("Inner error is " + ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				logger.Error("Problem handled", ex);
			}

			Listener.AssertContains("testsrule", "Castle.Core.Tests.TraceLoggerTests");
			Listener.AssertContains("testsrule", "Problem handled");
			Listener.AssertContains("testsrule", "Exception");
			Listener.AssertContains("testsrule", "Inner error is");
			Listener.AssertContains("testsrule", "ArgumentOutOfRangeException");
			Listener.AssertContains("testsrule", "fakearg");
			Listener.AssertContains("testsrule", "Thisisavalue");
			Listener.AssertContains("testsrule", "Thisisamessage");
		}

		[Test]
		public void WritingToLoggerByType()
		{
			var factory = new TraceLoggerFactory();
			var logger = factory.Create(typeof(TraceLoggerTests), LoggerLevel.Debug);
			logger.Debug("this is a tracing message");

			Listener.AssertContains("testsrule", "Castle.Core.Tests.TraceLoggerTests");
			Listener.AssertContains("testsrule", "this is a tracing message");
		}
	}
}