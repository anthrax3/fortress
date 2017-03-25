using System;
using Castle.Core.Core.Logging;
using NUnit.Framework;

namespace Castle.Core.Tests
{
	[TestFixture]
	public class Logging2TestCase
	{
		[SetUp]
		public void SetUp()
		{
			logger = new LevelFilteredLoggerInstance(this);

			// setting the default level to debug to simplify
			// the tests.

			logger.Level = LoggerLevel.Debug;

			// setting the level to an undefined value so
			// we dont have to wonder if it changed from Off
			// to Off (for instance).
			level = (LoggerLevel) (-1);
			name = null;
			message = null;
			exception = null;
			calls = 0;
		}

		private LevelFilteredLogger logger;

		internal LoggerLevel level;
		internal string name;
		internal string message;
		internal Exception exception;
		internal int calls;

		private void ValidateCall(LoggerLevel expectedLevel, string expectedMessage, Exception expectedException)
		{
			Assert.AreEqual(1, calls, "LevelFilteredLogger.Log was not called the right number of times");
			Assert.AreEqual(expectedLevel, level, "LevelFilteredLogger.Log was not called with the right level");
			Assert.AreEqual(expectedMessage, message, "LevelFilteredLogger.Log was not called with the right message");
			Assert.AreSame(expectedException, exception, "LevelFilteredLogger.Log was not called with the right exception");
			Assert.AreEqual("unnamed", name, "LevelFilteredLogger.Log was not called with the right name");
		}

		private void ValidateNoCalls()
		{
			Assert.AreEqual(0, calls, "LevelFilteredLogger.Log was called with logging " + logger.Level);
		}

		[Test]
		public void Debug()
		{
			var message = "Debug message";
			var level = LoggerLevel.Debug;
			Exception exception = null;

			logger.Debug(message);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Debug("Test");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}


		[Test]
		public void DebugLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.DebugFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}


		[Test]
		public void DebugLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateCall(LoggerLevel.Debug, "Test", exception);
		}

		[Test]
		public void DebugLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void DebugLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void DebugWithArgs()
		{
			var message = "Debug message 3";
			var level = LoggerLevel.Debug;
			Exception exception = null;

			logger.DebugFormat("{0} {1} {2}", "Debug", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void DebugWithException()
		{
			var message = "Debug message 2";
			var level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Debug(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Error()
		{
			var message = "Error message";
			var level = LoggerLevel.Error;
			Exception exception = null;

			logger.Error(message);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void ErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}


		[Test]
		public void ErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void ErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Test]
		public void ErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Test]
		public void ErrorWithArgs()
		{
			var message = "Error message 3";
			var level = LoggerLevel.Error;
			Exception exception = null;

			logger.ErrorFormat("{0} {1} {2}", "Error", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void ErrorWithException()
		{
			var message = "Error message 2";
			var level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalError()
		{
			var message = "FatalError message";
			var level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.Fatal(message);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void FatalErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}


		[Test]
		public void FatalErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}


		[Test]
		public void FatalErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelError()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Fatal("Test");

			ValidateNoCalls();
		}

		[Test]
		public void FatalErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.FatalFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void FatalErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void FatalErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Test]
		public void FatalErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Test]
		public void FatalErrorWithArgs()
		{
			var message = "FatalError message 3";
			var level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.FatalFormat("{0} {1} {2}", "FatalError", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void FatalErrorWithException()
		{
			var message = "FatalError message 2";
			var level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Info()
		{
			var message = "Info message";
			var level = LoggerLevel.Info;
			Exception exception = null;

			logger.Info(message);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void InfoLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}


		[Test]
		public void InfoLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}


		[Test]
		public void InfoLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Test]
		public void InfoLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Test]
		public void InfoLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Test]
		public void InfoLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void InfoLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void InfoWithArgs()
		{
			var message = "Info message 3";
			var level = LoggerLevel.Info;
			Exception exception = null;

			logger.InfoFormat("{0} {1} {2}", "Info", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void InfoWithException()
		{
			var message = "Info message 2";
			var level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Info(message, exception);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void Warn()
		{
			var message = "Warn message";
			var level = LoggerLevel.Warn;
			Exception exception = null;

			logger.Warn(message);

			ValidateCall(level, message, exception);
		}


		[Test]
		public void WarnLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}


		[Test]
		public void WarnLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}


		[Test]
		public void WarnLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Test]
		public void WarnLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Test]
		public void WarnLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Test]
		public void WarnWithArgs()
		{
			var message = "Warn message 3";
			var level = LoggerLevel.Warn;
			Exception exception = null;

			logger.WarnFormat("{0} {1} {2}", "Warn", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Test]
		public void WarnWithException()
		{
			var message = "Warn message 2";
			var level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Warn(message, exception);

			ValidateCall(level, message, exception);
		}
	}
}