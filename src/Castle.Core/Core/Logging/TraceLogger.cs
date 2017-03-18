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
using System.Collections.Generic;
using System.Diagnostics;

namespace Castle.Core.Core.Logging
{
	public class TraceLogger : LevelFilteredLogger
	{
		private static readonly Dictionary<string, TraceSource> cache = new Dictionary<string, TraceSource>();

		private TraceSource traceSource;

		public TraceLogger(string name)
			: base(name)
		{
			Initialize();
			Level = MapLoggerLevel(traceSource.Switch.Level);
		}

		public TraceLogger(string name, LoggerLevel level)
			: base(name, level)
		{
			Initialize();
			Level = MapLoggerLevel(traceSource.Switch.Level);
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			return InternalCreateChildLogger(loggerName);
		}

		private ILogger InternalCreateChildLogger(string loggerName)
		{
			return new TraceLogger(string.Concat(Name, ".", loggerName), Level);
		}

		protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
		{
			if (exception == null)
			{
				traceSource.TraceEvent(MapTraceEventType(loggerLevel), 0, message);
			}
			else
			{
				traceSource.TraceData(MapTraceEventType(loggerLevel), 0, message, exception);
			}
		}

		private void Initialize()
		{
			lock (cache)
			{
				// because TraceSource is meant to be used as a static member, and because
				// building up the configuraion inheritance is non-trivial, the instances
				// themselves are cached for so multiple TraceLogger instances will reuse
				// the named TraceSources which have been created

				if (cache.TryGetValue(Name, out traceSource))
				{
					return;
				}

				var defaultLevel = MapSourceLevels(Level);
				traceSource = new TraceSource(Name, defaultLevel);

				// no further action necessary when the named source is configured
				if (IsSourceConfigured(traceSource))
				{
					cache.Add(Name, traceSource);
					return;
				}

				// otherwise hunt for a shorter source that been configured            
				var foundSource = new TraceSource("Default", defaultLevel);

				var searchName = ShortenName(Name);
				while (!string.IsNullOrEmpty(searchName))
				{
					var searchSource = new TraceSource(searchName, defaultLevel);
					if (IsSourceConfigured(searchSource))
					{
						foundSource = searchSource;
						break;
					}

					searchName = ShortenName(searchName);
				}

				// reconfigure the created source to act like the found source
				traceSource.Switch = foundSource.Switch;
				traceSource.Listeners.Clear();
				foreach (TraceListener listener in foundSource.Listeners)
				{
					traceSource.Listeners.Add(listener);
				}

				cache.Add(Name, traceSource);
			}
		}

		private static string ShortenName(string name)
		{
			var lastDot = name.LastIndexOf('.');
			if (lastDot != -1)
			{
				return name.Substring(0, lastDot);
			}
			return null;
		}

		private static bool IsSourceConfigured(TraceSource source)
		{
			if (source.Listeners.Count == 1 &&
			    source.Listeners[0] is DefaultTraceListener &&
			    source.Listeners[0].Name == "Default")
			{
				return false;
			}
			return true;
		}

		private static LoggerLevel MapLoggerLevel(SourceLevels level)
		{
			switch (level)
			{
				case SourceLevels.All:
					return LoggerLevel.Debug;
				case SourceLevels.Verbose:
					return LoggerLevel.Debug;
				case SourceLevels.Information:
					return LoggerLevel.Info;
				case SourceLevels.Warning:
					return LoggerLevel.Warn;
				case SourceLevels.Error:
					return LoggerLevel.Error;
				case SourceLevels.Critical:
					return LoggerLevel.Fatal;
			}
			return LoggerLevel.Off;
		}

		private static SourceLevels MapSourceLevels(LoggerLevel level)
		{
			switch (level)
			{
				case LoggerLevel.Debug:
					return SourceLevels.Verbose;
				case LoggerLevel.Info:
					return SourceLevels.Information;
				case LoggerLevel.Warn:
					return SourceLevels.Warning;
				case LoggerLevel.Error:
					return SourceLevels.Error;
				case LoggerLevel.Fatal:
					return SourceLevels.Critical;
			}
			return SourceLevels.Off;
		}

		private static TraceEventType MapTraceEventType(LoggerLevel level)
		{
			switch (level)
			{
				case LoggerLevel.Debug:
					return TraceEventType.Verbose;
				case LoggerLevel.Info:
					return TraceEventType.Information;
				case LoggerLevel.Warn:
					return TraceEventType.Warning;
				case LoggerLevel.Error:
					return TraceEventType.Error;
				case LoggerLevel.Fatal:
					return TraceEventType.Critical;
			}
			return TraceEventType.Verbose;
		}
	}
}
