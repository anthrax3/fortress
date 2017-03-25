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
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class PropertyTests
	{
		[SetUp]
		public void SetUp()
		{
			logger = new LevelFilteredLoggerInstance(null);
		}

		private LevelFilteredLogger logger;

		[Test]
		public void DefaultLevel()
		{
			Assert.AreEqual(LoggerLevel.Off, logger.Level, "Default LevelFilteredLogger.Level is not Off");
		}

		[Test]
		public void DefaultName()
		{
			Assert.AreEqual("unnamed", logger.Name, "Default LevelFilteredLogger.Name is not String.Empty");
		}

		[Test]
		public void Level()
		{
			// Set the level to all available levels,
			// and then check that it was properly set
			foreach (LoggerLevel level in Enum.GetValues(typeof(LoggerLevel)))
			{
				logger.Level = level;
				Assert.AreEqual(level, logger.Level, "LevelFilteredLogger.Level did not change");
			}
		}

		[Test]
		public void LevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			Assert.IsTrue(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Debug");
			Assert.IsTrue(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Debug");
		}

		[Test]
		public void LevelError()
		{
			logger.Level = LoggerLevel.Error;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Error");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Error");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Error");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Error");
			Assert.IsTrue(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Error");
		}

		[Test]
		public void LevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Fatal");
			Assert.IsFalse(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Fatal");
			Assert.IsTrue(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Fatal");
		}

		[Test]
		public void LevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Info");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Info");
			Assert.IsTrue(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Info");
		}

		[Test]
		public void LevelOff()
		{
			logger.Level = LoggerLevel.Off;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning false when the level is Off");
			Assert.IsFalse(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning false when the level is Off");
		}

		[Test]
		public void LevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			Assert.IsFalse(logger.IsDebugEnabled, "LevelFilteredLogger.IsDebugEnabled is not returning false when the level is Warn");
			Assert.IsFalse(logger.IsInfoEnabled, "LevelFilteredLogger.IsInfoEnabled is not returning false when the level is Warn");
			Assert.IsTrue(logger.IsWarnEnabled, "LevelFilteredLogger.IsWarnEnabled is not returning true when the level is Warn");
			Assert.IsTrue(logger.IsErrorEnabled, "LevelFilteredLogger.IsErrorEnabled is not returning true when the level is Warn");
			Assert.IsTrue(logger.IsFatalEnabled, "LevelFilteredLogger.IsFatalErrorEnabled is not returning true when the level is Warn");
		}

		[Test]
		public void Name()
		{
			((LevelFilteredLoggerInstance) logger).ChangeName("Main");
			Assert.AreEqual("Main", logger.Name, "LevelFilteredLogger.Name did not change");

			((LevelFilteredLoggerInstance) logger).ChangeName("GUI");
			Assert.AreEqual("GUI", logger.Name, "LevelFilteredLogger.Name did not change");
		}

		[Test]
		public void SettingNameToNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				((LevelFilteredLoggerInstance) logger).ChangeName(null)
			);
		}
	}
}