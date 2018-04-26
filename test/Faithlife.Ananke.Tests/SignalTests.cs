using System;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Ananke.Tests.Util;
using NUnit.Framework;
// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable MethodSupportsCancellation

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
			var cancellationTokenSourceSet = new ManualResetEvent(false);
		    CancellationToken exitRequested;
		    Task.Run(() => Runner.Main(settings, context =>
		    {
			    exitRequested = context.ExitRequested;
			    ready.Set();
			    finish.WaitOne();
		    }));
		    ready.WaitOne();
		    exitRequested.Register(() => cancellationTokenSourceSet.Set());
		    Task.Run(() => settings.StubSigtermSignalService.Invoke());

		    cancellationTokenSourceSet.WaitOne();
		    Assert.That(exitRequested.IsCancellationRequested, Is.True);

		    finish.Set();
	    }

	    [Test]
	    public async Task Sigterm_BlocksHandlerUntilAppExits()
	    {
			var settings = new StubbedSettings();

		    var ready = new ManualResetEvent(false);
		    var finish = new ManualResetEvent(false);
		    var mainTask = Task.Run(() => Runner.Main(settings, context =>
		    {
			    ready.Set();
			    finish.WaitOne();
		    }));
		    ready.WaitOne();

			// The signal handler should block until mainTask completes.
		    var signalTask = Task.Run(() => settings.StubSigtermSignalService.Invoke());
		    var signalTaskCompleted = signalTask.Wait(TimeSpan.FromMilliseconds(200));
			Assert.That(signalTaskCompleted, Is.False);

		    finish.Set();
		    await mainTask;
		    await signalTask;
	    }
    }
}