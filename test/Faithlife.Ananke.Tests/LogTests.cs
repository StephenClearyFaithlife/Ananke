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
		    AnankeRunner.Main(settings, (Action<AnankeContext>) (_ => throw new InvalidOperationException()));
		    Assert.That(settings.StubStringLog.Messages, Has.None.Contains("\n"));
	    }

	    [Test]
	    public void EscapedConsoleStdout_EscapesEolCharacters()
	    {
		    var settings = new StubbedSettings();
		    AnankeRunner.Main(settings, context => context.EscapingConsoleStdout.WriteLine("Test\nMessage"));
			Assert.That(settings.StubStringLog.StringWriter.ToString(), Does.Contain("Test\\nMessage" + Environment.NewLine));
	    }
	}
}