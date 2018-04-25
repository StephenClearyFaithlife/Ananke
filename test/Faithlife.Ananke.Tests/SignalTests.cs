using System;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Ananke.Tests.Util;
using NUnit.Framework;

namespace Faithlife.Ananke.Tests
{
    public class SignalTests
    {
	    [Test]
	    public void Sigint_SignalsExitRequested()
	    {
		    var settings = new StubbedSettings();

			var ready = new ManualResetEvent(false);
			var finish = new ManualResetEvent(false);
		    CancellationToken exitRequested;
		    Task.Run(() => Runner.Main(settings, context =>
		    {
			    exitRequested = context.ExitRequested;
			    ready.Set();
			    finish.WaitOne();
		    }));
			ready.WaitOne();

			settings.StubSigintSignalService.Invoke();
			Assert.That(exitRequested.IsCancellationRequested, Is.True);

		    finish.Set();
	    }

	    [Test]
	    public void Sigterm_SignalsExitRequested()
	    {
		    var settings = new StubbedSettings();

		    var ready = new ManualResetEvent(false);
		    var finish = new ManualResetEvent(false);
		    CancellationToken exitRequested;
		    Task.Run(() => Runner.Main(settings, context =>
		    {
			    exitRequested = context.ExitRequested;
			    ready.Set();
			    finish.WaitOne();
		    }));
		    ready.WaitOne();

		    settings.StubSigtermSignalService.Invoke();
		    Assert.That(exitRequested.IsCancellationRequested, Is.True);

		    finish.Set();
	    }
    }
}