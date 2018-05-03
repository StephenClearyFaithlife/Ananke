using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Faithlife.Ananke.Logging;
using Faithlife.Ananke.Tests.Util;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Faithlife.Ananke.Tests
{
    public class LogTests
    {
	    [Test]
	    public void ExceptionLogging_EscapesEolCharacters()
	    {
		    var settings = new StubbedSettings();
		    AnankeRunner.Main(settings, (Action<AnankeContext>) (_ => throw new InvalidOperationException("test message")));
		    Assert.That(settings.StubStringLog.Messages, Has.Some.Contains("test message"));
		    Assert.That(settings.StubStringLog.Messages, Has.None.Contains("\n"));
	    }

	    [Test]
	    public void EscapedConsoleStdout_EscapesEolCharacters()
	    {
		    var settings = new StubbedSettings();
		    AnankeRunner.Main(settings, context => context.EscapingConsoleStdout.WriteLine("Test\nMessage"));
			Assert.That(settings.StubStringLog.StringWriter.ToString(), Does.Contain("Test\\nMessage" + Environment.NewLine));
	    }

		[Test]
	    public void CustomFormatter_GetsStructuredState()
		{
			var actualState = new List<KeyValuePair<string, object>>();
			string TestFormatter(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception,
				IEnumerable<KeyValuePair<string, object>> state, IEnumerable<IEnumerable<KeyValuePair<string, object>>> scope)
			{
				actualState.AddRange(state);
				return message;
			}

			var settings = new StubbedSettings
			{
				Formatter = TestFormatter,
			};
			var timestamp = new DateTime(2018, 06, 01, 0, 0, 0, DateTimeKind.Utc);
			AnankeRunner.Main(settings,
				context => context.LoggerProvider.CreateLogger("testlogger").LogInformation("Hello from {source} at {timestamp:o}!", "sourceValue", timestamp));

			Assert.That(settings.StubStringLog.Messages, Has.Some.Contains("Hello from sourceValue at 2018-06-01T00:00:00.0000000Z!"));
			Assert.That(actualState, Has.Some.EqualTo(new KeyValuePair<string, object>("source", "sourceValue")));
			Assert.That(actualState, Has.Some.EqualTo(new KeyValuePair<string, object>("timestamp", timestamp)));
		}
	}
}