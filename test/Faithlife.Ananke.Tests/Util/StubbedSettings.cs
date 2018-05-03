using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Faithlife.Ananke.Logging;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubbedSettings
    {
	    public TimeSpan StubMaximumRuntime { get; set; } = Timeout.InfiniteTimeSpan;

	    public TimeSpan StubExitTimeout { get; set; } = TimeSpan.FromSeconds(1);

	    public AnankeLoggerProvider.Formatter Formatter { get; set; } = null;

	    public StubStringLog StubStringLog { get; } = new StubStringLog(new StringWriter());

	    public StubExitProcessService StubExitProcessService { get; } = new StubExitProcessService();

		public StubSignalService StubSignalService { get; } = new StubSignalService();

	    public static implicit operator AnankeSettings(StubbedSettings stubs)
	    {
			var result = AnankeSettings.Create(maximumRuntime: stubs.StubMaximumRuntime, exitTimeout: stubs.StubExitTimeout,
				consoleLog: stubs.StubStringLog, anankeLoggerProviderFormatter: stubs.Formatter);
		    result.ExitProcessService = stubs.StubExitProcessService;
		    result.SignalService = stubs.StubSignalService;
		    return result;
	    }
	}
}
