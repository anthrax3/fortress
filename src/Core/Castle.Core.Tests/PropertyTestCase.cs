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
using Castle.Core.Core.Logging;
using Xunit;


namespace Castle.Core.Tests
{
	public class PropertyTestCase
	{
		public PropertyTestCase()
		{
			logger = new LevelFilteredLoggerInstance(null);
		}

		private LevelFilteredLogger logger;

		[Fact]
		public void DefaultLevel()
		{
			Assert.Equal(LoggerLevel.Off, logger.Level);
		}

		[Fact]
		public void DefaultName()
		{
			Assert.Equal("unnamed", logger.Name);
		}

		[Fact]
		public void Level()
		{
			foreach (LoggerLevel level in Enum.GetValues(typeof(LoggerLevel)))
			{
				logger.Level = level;
				Assert.Equal(level, logger.Level);
			}
		}

		[Fact]
		public void LevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			Assert.True(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Debug");
			Assert.True(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Debug");
		}

		[Fact]
		public void LevelError()
		{
			logger.Level = LoggerLevel.Error;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Error");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Error");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Error");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Error");
			Assert.True(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Error");
		}

		[Fact]
		public void LevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Fatal");
			Assert.False(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Fatal");
			Assert.True(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Fatal");
		}

		[Fact]
		public void LevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Info");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Info");
			Assert.True(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Info");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Info");
			Assert.True(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Info");
		}

		[Fact]
		public void LevelOff()
		{
			logger.Level = LoggerLevel.Off;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Off");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Off");
			Assert.False(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Off");
			Assert.False(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Off");
			Assert.False(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning false when the level is Off");
		}

		[Fact]
		public void LevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			Assert.False(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Warn");
			Assert.False(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Warn");
			Assert.True(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Warn");
			Assert.True(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Warn");
			Assert.True(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Warn");
		}

		[Fact]
		public void Name()
		{
			((LevelFilteredLoggerInstance) logger).ChangeName("Main");
			Assert.Equal("Main", logger.Name);

			((LevelFilteredLoggerInstance) logger).ChangeName("GUI");
			Assert.Equal("GUI", logger.Name);
		}

		[Fact]
		public void SettingNameToNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				((LevelFilteredLoggerInstance) logger).ChangeName(null)
			);
		}
	}
}