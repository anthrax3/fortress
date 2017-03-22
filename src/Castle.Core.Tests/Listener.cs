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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	public class TraceLoggerTests
	{
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
	}
}