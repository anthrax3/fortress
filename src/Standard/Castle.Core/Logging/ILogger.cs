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

namespace Castle.Core.Logging
{
	public interface ILogger
	{
		bool IsDebugEnabled { get; }

		bool IsErrorEnabled { get; }

		bool IsFatalEnabled { get; }

		bool IsInfoEnabled { get; }

		bool IsWarnEnabled { get; }

		ILogger CreateChildLogger(string loggerName);

		void Debug(string message);

		void Debug(Func<string> messageFactory);

		void Debug(string message, Exception exception);

		void DebugFormat(string format, params object[] args);

		void DebugFormat(Exception exception, string format, params object[] args);

		void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);

		void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

		void Error(string message);

		void Error(Func<string> messageFactory);

		void Error(string message, Exception exception);

		void ErrorFormat(string format, params object[] args);

		void ErrorFormat(Exception exception, string format, params object[] args);

		void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);

		void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

		void Fatal(string message);

		void Fatal(Func<string> messageFactory);

		void Fatal(string message, Exception exception);

		void FatalFormat(string format, params object[] args);

		void FatalFormat(Exception exception, string format, params object[] args);

		void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);

		void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

		void Info(string message);

		void Info(Func<string> messageFactory);

		void Info(string message, Exception exception);

		void InfoFormat(string format, params object[] args);

		void InfoFormat(Exception exception, string format, params object[] args);

		void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);

		void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

		void Warn(string message);

		void Warn(Func<string> messageFactory);

		void Warn(string message, Exception exception);

		void WarnFormat(string format, params object[] args);

		void WarnFormat(Exception exception, string format, params object[] args);

		void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);

		void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
	}
}