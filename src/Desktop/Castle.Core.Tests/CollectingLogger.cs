using System;
using System.Collections.Generic;
using Castle.Core.Logging;

namespace Castle.Core.Tests
{
	public class CollectingLogger : ILogger
	{
		private readonly List<string> messages = new List<string>();

		public void Debug(string message)
		{
			throw new NotImplementedException();
		}

		public void Debug(Func<string> messageFactory)
		{
			throw new NotImplementedException();
		}

		public void Debug(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(string format, params object[] args)
		{
			messages.Add("DEBUG: " + string.Format(format, args));
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Info(string message)
		{
			throw new NotImplementedException();
		}

		public void Info(Func<string> messageFactory)
		{
			throw new NotImplementedException();
		}

		public void Info(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Warn(string message)
		{
			throw new NotImplementedException();
		}

		public void Warn(Func<string> messageFactory)
		{
			throw new NotImplementedException();
		}

		public void Warn(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, params object[] args)
		{
			messages.Add("WARN: " + string.Format(format, args));
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Error(string message)
		{
			throw new NotImplementedException();
		}

		public void Error(Func<string> messageFactory)
		{
			throw new NotImplementedException();
		}

		public void Error(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Fatal(string message)
		{
			throw new NotImplementedException();
		}

		public void Fatal(Func<string> messageFactory)
		{
			throw new NotImplementedException();
		}

		public void Fatal(string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public ILogger CreateChildLogger(string loggerName)
		{
			throw new NotImplementedException();
		}

		public bool IsDebugEnabled
		{
			get { return true; }
		}

		public bool IsInfoEnabled
		{
			get { return true; }
		}

		public bool IsWarnEnabled
		{
			get { return true; }
		}

		public bool IsErrorEnabled
		{
			get { return true; }
		}

		public bool IsFatalEnabled
		{
			get { return true; }
		}

		public bool RecordedMessage(LoggerLevel level, string message)
		{
			return messages.Contains(level.ToString().ToUpper() + ": " + message);
		}
	}
}