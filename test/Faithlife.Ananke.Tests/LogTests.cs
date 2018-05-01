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
	    public void EscapingStdout_EscapesEolCharacters()
	    {
			var settings = new StubbedSettings();
		    AnankeRunner.Main(settings, context => context.EscapedConsoleStdout.WriteLine("Test\nMessage"));
			Assert.That(settings.StubConsoleStdout.ToString(), Is.EqualTo("Test\\nMessage" + Environment.NewLine));
	    }

	    [Test]
	    public void EscapingStderr_EscapesEolCharacters()
	    {
		    var settings = new StubbedSettings();
		    AnankeRunner.Main(settings, context => context.EscapedConsoleStderr.WriteLine("Test\nMessage"));
		    Assert.That(settings.StubConsoleStderr.ToString(), Is.EqualTo("Test\\nMessage" + Environment.NewLine));
	    }
	}
}