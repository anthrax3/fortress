// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Logging
{
	public abstract class LevelFilteredLogger : ILogger
	{
		private LoggerLevel level = LoggerLevel.Off;

		protected LevelFilteredLogger()
		{
		}

		protected LevelFilteredLogger(string name)
		{
			ChangeName(name);
		}

		protected LevelFilteredLogger(LoggerLevel loggerLevel)
		{
			level = loggerLevel;
		}

		protected LevelFilteredLogger(string loggerName, LoggerLevel loggerLevel) : this(loggerLevel)
		{
			ChangeName(loggerName);
		}

		public LoggerLevel Level
		{
			get { return level; }
			set { level = value; }
		}

		public string Name { get; private set; } = "unnamed";

		public abstract ILogger CreateChildLogger(string loggerName);

		protected abstract void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception);

		protected void ChangeName(string newName)
		{
			if (newName == null)
				throw new ArgumentNullException("newName");

			Name = newName;
		}

		private void Log(LoggerLevel loggerLevel, string message, Exception exception)
		{
			Log(loggerLevel, Name, message, exception);
		}

		#region ILogger implementation

		#region Debug

		public void Debug(string message)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, message, null);
		}

		public void Debug(Func<string> messageFactory)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, messageFactory.Invoke(), null);
		}

		public void Debug(string message, Exception exception)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		public void DebugFormat(Exception exception, string format, params object[] args)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, string.Format(formatProvider, format, args), null);
		}

		public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsDebugEnabled)
				return;

			Log(LoggerLevel.Debug, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Info

		public void Info(string message)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, message, null);
		}

		public void Info(Func<string> messageFactory)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, messageFactory.Invoke(), null);
		}

		public void Info(string message, Exception exception)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		public void InfoFormat(Exception exception, string format, params object[] args)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, string.Format(formatProvider, format, args), null);
		}

		public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsInfoEnabled)
				return;

			Log(LoggerLevel.Info, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Warn

		public void Warn(string message)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, message, null);
		}

		public void Warn(Func<string> messageFactory)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, messageFactory.Invoke(), null);
		}

		public void Warn(string message, Exception exception)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		public void WarnFormat(Exception exception, string format, params object[] args)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, string.Format(formatProvider, format, args), null);
		}

		public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsWarnEnabled)
				return;

			Log(LoggerLevel.Warn, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Error

		public void Error(string message)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, message, null);
		}

		public void Error(Func<string> messageFactory)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, messageFactory.Invoke(), null);
		}

		public void Error(string message, Exception exception)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		public void ErrorFormat(Exception exception, string format, params object[] args)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, string.Format(formatProvider, format, args), null);
		}

		public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsErrorEnabled)
				return;

			Log(LoggerLevel.Error, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		#region Fatal

		public void Fatal(string message)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, message, null);
		}

		public void Fatal(Func<string> messageFactory)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, messageFactory.Invoke(), null);
		}

		public void Fatal(string message, Exception exception)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, message, exception);
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), null);
		}

		public void FatalFormat(Exception exception, string format, params object[] args)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, string.Format(CultureInfo.CurrentCulture, format, args), exception);
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, string.Format(formatProvider, format, args), null);
		}

		public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
		{
			if (!IsFatalEnabled)
				return;

			Log(LoggerLevel.Fatal, string.Format(formatProvider, format, args), exception);
		}

		#endregion

		public bool IsDebugEnabled
		{
			get { return Level >= LoggerLevel.Debug; }
		}

		public bool IsInfoEnabled
		{
			get { return Level >= LoggerLevel.Info; }
		}

		public bool IsWarnEnabled
		{
			get { return Level >= LoggerLevel.Warn; }
		}

		public bool IsErrorEnabled
		{
			get { return Level >= LoggerLevel.Error; }
		}

		public bool IsFatalEnabled
		{
			get { return Level >= LoggerLevel.Fatal; }
		}

		#endregion
	}
}