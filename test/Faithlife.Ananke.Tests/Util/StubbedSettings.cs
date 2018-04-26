using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubbedSettings
    {
	    public StubStringLogService StubStringLogService { get; } = new StubStringLogService();

	    public StubExitProcessService StubExitProcessService { get; } = new StubExitProcessService();

		public StubSignalService StubSigintSignalService { get; } = new StubSignalService();

		public StubSignalService StubSigtermSignalService { get; } = new StubSignalService();

		public StringWriter StubConsoleStdout { get; } = new StringWriter();

		public StringWriter StubConsoleStderr { get; } = new StringWriter();

	    public static implicit operator AnankeSettings(StubbedSettings stubs)
	    {
			return AnankeSettings.Create(consoleLogService: stubs.StubStringLogService, exitProcessService: stubs.StubExitProcessService,
				sigintSignalService: stubs.StubSigintSignalService, sigtermSignalService: stubs.StubSigtermSignalService,
				consoleStdout: stubs.StubConsoleStdout, consoleStderr: stubs.StubConsoleStderr);
	    }
	}
}
