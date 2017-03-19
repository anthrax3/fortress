// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
using System.Globalization;

namespace Castle.Core.Core.Logging
{
	[Serializable]
	public class ConsoleLogger : LevelFilteredLogger
	{
		public ConsoleLogger() : this(String.Empty, LoggerLevel.Debug)
		{
		}

		public ConsoleLogger(LoggerLevel logLevel) : this(String.Empty, logLevel)
		{
		}

		public ConsoleLogger(String name) : this(name, LoggerLevel.Debug)
		{
		}

		public ConsoleLogger(String name, LoggerLevel logLevel) : base(name, logLevel)
		{
		}

		protected override void Log(LoggerLevel loggerLevel, String loggerName, String message, Exception exception)
		{
			Console.Out.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, message);

			if (exception != null)
			{
				Console.Out.WriteLine("[{0}] '{1}' {2}: {3} {4}", loggerLevel, loggerName, exception.GetType().FullName,
				                      exception.Message, exception.StackTrace);
			}
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			if (loggerName == null)
			{
				throw new ArgumentNullException("loggerName", "To create a child logger you must supply a non null name");
			}

			return new ConsoleLogger(String.Format(CultureInfo.CurrentCulture, "{0}.{1}", Name, loggerName), Level);
		}
	}
}
