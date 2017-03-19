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

namespace Castle.Core.Core.Logging
{
	public interface ILogger
	{
		bool IsDebugEnabled { get; }

		bool IsErrorEnabled { get; }

		bool IsFatalEnabled { get; }

		bool IsInfoEnabled { get; }

		bool IsWarnEnabled { get; }

		ILogger CreateChildLogger(String loggerName);

		void Debug(String message);

		void Debug(Func<string> messageFactory);

		void Debug(String message, Exception exception);

		void DebugFormat(String format, params Object[] args);

		void DebugFormat(Exception exception, String format, params Object[] args);

		void DebugFormat(IFormatProvider formatProvider, String format, params Object[] args);

		void DebugFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		void Error(String message);

		void Error(Func<string> messageFactory);

		void Error(String message, Exception exception);

		void ErrorFormat(String format, params Object[] args);

		void ErrorFormat(Exception exception, String format, params Object[] args);

		void ErrorFormat(IFormatProvider formatProvider, String format, params Object[] args);

		void ErrorFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		void Fatal(String message);

		void Fatal(Func<string> messageFactory);

		void Fatal(String message, Exception exception);

		void FatalFormat(String format, params Object[] args);

		void FatalFormat(Exception exception, String format, params Object[] args);

		void FatalFormat(IFormatProvider formatProvider, String format, params Object[] args);

		void FatalFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		void Info(String message);

		void Info(Func<string> messageFactory);

		void Info(String message, Exception exception);

		void InfoFormat(String format, params Object[] args);

		void InfoFormat(Exception exception, String format, params Object[] args);

		void InfoFormat(IFormatProvider formatProvider, String format, params Object[] args);

		void InfoFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);

		void Warn(String message);

		void Warn(Func<string> messageFactory);

		void Warn(String message, Exception exception);

		void WarnFormat(String format, params Object[] args);

		void WarnFormat(Exception exception, String format, params Object[] args);

		void WarnFormat(IFormatProvider formatProvider, String format, params Object[] args);

		void WarnFormat(Exception exception, IFormatProvider formatProvider, String format, params Object[] args);
	}
}
