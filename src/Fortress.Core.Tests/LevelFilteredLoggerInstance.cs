using System;
using Castle.Core.Logging;

namespace Castle.Core.Tests
{
	internal class LevelFilteredLoggerInstance : LevelFilteredLogger
	{
		private readonly Logging2TestCase Fixture;

		public LevelFilteredLoggerInstance(Logging2TestCase fixture)
		{
			Fixture = fixture;
		}

		public new void ChangeName(string name)
		{
			base.ChangeName(name);
		}

		protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
		{
			Fixture.level = loggerLevel;
			Fixture.name = loggerName;
			Fixture.message = message;
			Fixture.exception = exception;

			Fixture.calls++;
		}

		public override ILogger CreateChildLogger(string loggerName)
		{
			return null;
		}
	}
}