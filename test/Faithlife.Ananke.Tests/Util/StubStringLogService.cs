using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubStringLogService : IStringLogService
    {
		public List<string> Messages { get; } = new List<string>();

	    public void WriteLine(string message)
	    {
		    Messages.Add(message);
		}
    }
}
