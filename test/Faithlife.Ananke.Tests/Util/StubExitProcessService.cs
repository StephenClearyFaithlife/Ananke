using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubExitProcessService : IExitProcessService
    {
		public int? ExitCode { get; private set; }

	    public int Exit(int exitCode)
	    {
		    ExitCode = exitCode;
		    return exitCode;
	    }
    }
}
