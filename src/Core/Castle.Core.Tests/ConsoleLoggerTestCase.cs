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
using Castle.Core.Logging;
using Xunit;

namespace Castle.Core.Tests
{
	public class ConsoleLoggerTestCase : IDisposable
	{
		public ConsoleLoggerTestCase()
		{
			outWriter.GetStringBuilder().Length = 0;
			errorWriter.GetStringBuilder().Length = 0;
			oldOut = Console.Out;
			oldError = Console.Error;
			Console.SetOut(outWriter);
			Console.SetError(errorWriter);
		}

		public void Dispose()
		{
			Console.SetOut(oldOut);
			Console.SetError(oldError);
		}

		private readonly StringWriter outWriter = new StringWriter();
		private readonly StringWriter errorWriter = new StringWriter();
		private TextWriter oldOut;
		private TextWriter oldError;

		[Fact]
		public void DebugLogger()
		{
			var log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			var logcontents = outWriter.GetStringBuilder().ToString();

			var expected = new StringWriter();
			expected.WriteLine("[Debug] 'Logger' Some debug message");
			expected.WriteLine("[Info] 'Logger' Some info message");
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents);
		}

		[Fact]
		public void ExceptionLogging()
		{
			var log = new ConsoleLogger("Logger", LoggerLevel.Debug);

			log.Debug("Some debug message", new Exception("Some exception message"));

			var logcontents = outWriter.GetStringBuilder().ToString();

			var expected = new StringWriter();
			expected.WriteLine("[Debug] 'Logger' Some debug message");
			expected.WriteLine("[Debug] 'Logger' System.Exception: Some exception message ");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents);
		}

		[Fact]
		public void InfoLogger()
		{
			var log = new ConsoleLogger("Logger", LoggerLevel.Info);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			var logcontents = outWriter.GetStringBuilder().ToString();

			var expected = new StringWriter();
			expected.WriteLine("[Info] 'Logger' Some info message");
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents);
		}

		[Fact]
		public void WarnLogger()
		{
			var log = new ConsoleLogger("Logger", LoggerLevel.Warn);

			log.Debug("Some debug message");
			log.Info("Some info message");
			log.Error("Some error message");
			log.Fatal("Some fatal error message");
			log.Warn("Some warn message");

			var logcontents = outWriter.GetStringBuilder().ToString();

			var expected = new StringWriter();
			expected.WriteLine("[Error] 'Logger' Some error message");
			expected.WriteLine("[Fatal] 'Logger' Some fatal error message");
			expected.WriteLine("[Warn] 'Logger' Some warn message");

			Assert.Equal(expected.GetStringBuilder().ToString(), logcontents);
		}
	}
}