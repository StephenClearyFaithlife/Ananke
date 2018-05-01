﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubbedSettings
    {
	    public TimeSpan StubMaximumRuntime { get; set; } = Timeout.InfiniteTimeSpan;

	    public TimeSpan StubExitTimeout { get; set; } = TimeSpan.FromSeconds(1);

	    public StubStringLog StubStringLog { get; } = new StubStringLog();

	    public StubExitProcessService StubExitProcessService { get; } = new StubExitProcessService();

		public StubSignalService StubSignalService { get; } = new StubSignalService();

		public StringWriter StubConsoleStdout { get; } = new StringWriter();

		public StringWriter StubConsoleStderr { get; } = new StringWriter();

	    public static implicit operator AnankeSettings(StubbedSettings stubs)
	    {
			return AnankeSettings.Create(maximumRuntime: stubs.StubMaximumRuntime, exitTimeout: stubs.StubExitTimeout,
				consoleLog: stubs.StubStringLog, exitProcessService: stubs.StubExitProcessService,
				signalService: stubs.StubSignalService, consoleStdout: stubs.StubConsoleStdout, consoleStderr: stubs.StubConsoleStderr);
	    }
	}
}
