using System;
using Faithlife.Ananke.Tests.Util;
using NUnit.Framework;

namespace Faithlife.Ananke.Tests
{
    public class LogTests
    {
	    [Test]
	    public void ExceptionLogging_EscapesEolCharacters()
	    {
		    var settings = new StubbedSettings();
		    Runner.Main(settings, (Action<Context>) (_ => throw new InvalidOperationException()));
		    Assert.That(settings.StubStringLogService.Messages, Has.None.Contains("\n"));
	    }
	}
}