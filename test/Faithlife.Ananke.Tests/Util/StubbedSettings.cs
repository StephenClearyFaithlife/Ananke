using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubbedSettings
    {
	    public StubStringLogService StubStringLogService { get; } = new StubStringLogService();

	    public StubExitProcessService StubExitProcessService { get; } = new StubExitProcessService();

	    public static implicit operator Settings(StubbedSettings stubs)
	    {
			return Settings.Create(consoleLogService: stubs.StubStringLogService,
				exitProcessService: stubs.StubExitProcessService);
	    }
	}
}
