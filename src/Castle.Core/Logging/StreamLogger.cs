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
using System.IO;
using System.Text;

namespace Castle.Core.Core.Logging
{
	[Serializable]
	public class StreamLogger : LevelFilteredLogger, IDisposable
	{
		private StreamWriter writer;

		public StreamLogger(string name, Stream stream) : this(name, new StreamWriter(stream))
		{
		}

		public StreamLogger(string name, Stream stream, Encoding encoding) : this(name, new StreamWriter(stream, encoding))
		{
		}

		public StreamLogger(string name, Stream stream, Encoding encoding, int bufferSize)
			: this(name, new StreamWriter(stream, encoding, bufferSize))
		{
		}

		protected StreamLogger(string name, StreamWriter writer) : base(name, LoggerLevel.Debug)
		{
			this.writer = writer;
			writer.AutoFlush = true;
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~StreamLogger()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				if (writer != null)
				{
					writer.Dispose();
					writer = null;
				}
		}

		protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
		{
			if (writer == null)
				return; // just in case it's been disposed

			writer.WriteLine("[{0}] '{1}' {2}", loggerLevel, loggerName, message);

			if (exception != null)
				writer.WriteLine("[{0}] '{1}' {2}: {3} {4}",
					loggerLevel,
					loggerName,
					exception.GetType().FullName,
					exception.Message,
					exception.StackTrace);
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			// TODO: We could create a ChildStreamLogger that didn't take ownership of the stream

			throw new NotSupportedException("A streamlogger does not support child loggers");
		}
	}
}