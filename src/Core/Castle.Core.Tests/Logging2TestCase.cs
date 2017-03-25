using System;
using Castle.Core.Core.Logging;
using Xunit;


namespace Castle.Core.Tests
{
	public class Logging2TestCase
	{
		public Logging2TestCase()
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
			Assert.Equal(1, calls);
			Assert.Equal(expectedLevel, level);
			Assert.Equal(expectedMessage, message);
			Assert.Same(expectedException, exception);
			Assert.Equal("unnamed", name);
		}

		private void ValidateNoCalls()
		{
			Assert.Equal(0, calls);
		}

		[Fact]
		public void Debug()
		{
			var message = "Debug message";
			var level = LoggerLevel.Debug;
			Exception exception = null;

			logger.Debug(message);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void DebugLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Debug("Test");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}


		[Fact]
		public void DebugLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.DebugFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Debug, "Test", null);
		}


		[Fact]
		public void DebugLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateCall(LoggerLevel.Debug, "Test", exception);
		}

		[Fact]
		public void DebugLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Debug("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.DebugFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void DebugLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Debug("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void DebugWithArgs()
		{
			var message = "Debug message 3";
			var level = LoggerLevel.Debug;
			Exception exception = null;

			logger.DebugFormat("{0} {1} {2}", "Debug", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void DebugWithException()
		{
			var message = "Debug message 2";
			var level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Debug(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void Error()
		{
			var message = "Error message";
			var level = LoggerLevel.Error;
			Exception exception = null;

			logger.Error(message);

			ValidateCall(level, message, exception);
		}


		[Fact]
		public void ErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}


		[Fact]
		public void ErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Error("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.ErrorFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void ErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Error("Test");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.ErrorFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Error, "Test", null);
		}

		[Fact]
		public void ErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error("Test", exception);

			ValidateCall(LoggerLevel.Error, "Test", exception);
		}

		[Fact]
		public void ErrorWithArgs()
		{
			var message = "Error message 3";
			var level = LoggerLevel.Error;
			Exception exception = null;

			logger.ErrorFormat("{0} {1} {2}", "Error", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void ErrorWithException()
		{
			var message = "Error message 2";
			var level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Error(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void FatalError()
		{
			var message = "FatalError message";
			var level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.Fatal(message);

			ValidateCall(level, message, exception);
		}


		[Fact]
		public void FatalErrorLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}


		[Fact]
		public void FatalErrorLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}


		[Fact]
		public void FatalErrorLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelError()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Fatal("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void FatalErrorLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.FatalFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void FatalErrorLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void FatalErrorLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Fatal("Test");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.FatalFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Fatal, "Test", null);
		}

		[Fact]
		public void FatalErrorLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal("Test", exception);

			ValidateCall(LoggerLevel.Fatal, "Test", exception);
		}

		[Fact]
		public void FatalErrorWithArgs()
		{
			var message = "FatalError message 3";
			var level = LoggerLevel.Fatal;
			Exception exception = null;

			logger.FatalFormat("{0} {1} {2}", "FatalError", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void FatalErrorWithException()
		{
			var message = "FatalError message 2";
			var level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Fatal(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void Info()
		{
			var message = "Info message";
			var level = LoggerLevel.Info;
			Exception exception = null;

			logger.Info(message);

			ValidateCall(level, message, exception);
		}


		[Fact]
		public void InfoLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}


		[Fact]
		public void InfoLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}


		[Fact]
		public void InfoLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Fact]
		public void InfoLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Info("Test");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.InfoFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Info, "Test", null);
		}

		[Fact]
		public void InfoLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateCall(LoggerLevel.Info, "Test", exception);
		}

		[Fact]
		public void InfoLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Info("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.InfoFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void InfoLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Info("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void InfoWithArgs()
		{
			var message = "Info message 3";
			var level = LoggerLevel.Info;
			Exception exception = null;

			logger.InfoFormat("{0} {1} {2}", "Info", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void InfoWithException()
		{
			var message = "Info message 2";
			var level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Info(message, exception);

			ValidateCall(level, message, exception);
		}

		[Fact]
		public void Warn()
		{
			var message = "Warn message";
			var level = LoggerLevel.Warn;
			Exception exception = null;

			logger.Warn(message);

			ValidateCall(level, message, exception);
		}


		[Fact]
		public void WarnLevelDebug()
		{
			logger.Level = LoggerLevel.Debug;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}


		[Fact]
		public void WarnLevelDebugWithArgs()
		{
			logger.Level = LoggerLevel.Debug;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}


		[Fact]
		public void WarnLevelDebugWithException()
		{
			logger.Level = LoggerLevel.Debug;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnLevelError()
		{
			logger.Level = LoggerLevel.Error;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelErrorWithArgs()
		{
			logger.Level = LoggerLevel.Error;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelErrorWithException()
		{
			logger.Level = LoggerLevel.Error;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatal()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatalWithArgs()
		{
			logger.Level = LoggerLevel.Fatal;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelFatalWithException()
		{
			logger.Level = LoggerLevel.Fatal;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelInfo()
		{
			logger.Level = LoggerLevel.Info;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelInfoWithArgs()
		{
			logger.Level = LoggerLevel.Info;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelInfoWithException()
		{
			logger.Level = LoggerLevel.Info;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnLevelOff()
		{
			logger.Level = LoggerLevel.Off;

			logger.Warn("Test");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelOffWithArgs()
		{
			logger.Level = LoggerLevel.Off;

			logger.WarnFormat("{0}st", "Te");

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelOffWithException()
		{
			logger.Level = LoggerLevel.Off;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateNoCalls();
		}

		[Fact]
		public void WarnLevelWarn()
		{
			logger.Level = LoggerLevel.Warn;

			logger.Warn("Test");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelWarnWithArgs()
		{
			logger.Level = LoggerLevel.Warn;

			logger.WarnFormat("{0}st", "Te");

			ValidateCall(LoggerLevel.Warn, "Test", null);
		}

		[Fact]
		public void WarnLevelWarnWithException()
		{
			logger.Level = LoggerLevel.Warn;
			var exception = new Exception();

			logger.Warn("Test", exception);

			ValidateCall(LoggerLevel.Warn, "Test", exception);
		}

		[Fact]
		public void WarnWithArgs()
		{
			var message = "Warn message 3";
			var level = LoggerLevel.Warn;
			Exception exception = null;

			logger.WarnFormat("{0} {1} {2}", "Warn", "message", 3);

			ValidateCall(level, message, exception);
		}

		[Fact]
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