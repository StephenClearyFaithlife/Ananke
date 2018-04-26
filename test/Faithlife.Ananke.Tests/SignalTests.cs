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
		    Task.Run(() => AnankeRunner.Main(settings, context =>
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
		    Task.Run(() => AnankeRunner.Main(settings, context =>
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
		    var mainTask = Task.Run(() => AnankeRunner.Main(settings, context =>
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

	    [Test]
	    public async Task ExitRequestedCancellationException_WhenExitRequested_IsTreatedAsNormalException()
	    {
			var settings = new StubbedSettings();
			var ready = new ManualResetEventSlim();
		    var task = Task.Run(() => AnankeRunner.Main(settings, async context =>
		    {
			    ready.Set();
			    await Task.Delay(Timeout.InfiniteTimeSpan, context.ExitRequested);
		    }));
		    ready.Wait();

			settings.StubSigintSignalService.Invoke();

		    await task;
			Assert.That(settings.StubExitProcessService.ExitCode, Is.Zero);
	    }

	    [Test]
	    public async Task LinkedCancellationException_WhenExitRequested_IsTreatedAsNormalException()
	    {
		    var settings = new StubbedSettings();
		    var ready = new ManualResetEventSlim();
		    var task = Task.Run(() => AnankeRunner.Main(settings, async context =>
		    {
			    ready.Set();
				var otherCts = new CancellationTokenSource();
			    var cts = CancellationTokenSource.CreateLinkedTokenSource(context.ExitRequested, otherCts.Token);
			    await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
		    }));
		    ready.Wait();

		    settings.StubSigintSignalService.Invoke();

		    await task;
		    Assert.That(settings.StubExitProcessService.ExitCode, Is.Zero);
	    }

	    [Test]
	    public void Sigint_WhenApplicationCodeTakesTooLong_ExitsProcessWithCode65()
	    {
		    var settings = new StubbedSettings();

		    var ready = new ManualResetEventSlim();
			var finish = new ManualResetEventSlim();
		    Task.Run(() => AnankeRunner.Main(settings, context =>
		    {
			    ready.Set();
			    finish.Wait();
		    }));
		    ready.Wait();

			settings.StubSigintSignalService.Invoke();
		    Thread.Sleep(settings.StubExitTimeSpan * 2);
			Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(65));

		    finish.Set();
	    }

	    [Test]
	    public void Sigterm_WhenApplicationCodeTakesTooLong_ExitsProcessWithCode65()
	    {
		    var settings = new StubbedSettings();

		    var ready = new ManualResetEventSlim();
		    var finish = new ManualResetEventSlim();
		    Task.Run(() => AnankeRunner.Main(settings, context =>
		    {
			    ready.Set();
			    finish.Wait();
		    }));
		    ready.Wait();

		    Task.Run(() => settings.StubSigtermSignalService.Invoke());
		    Thread.Sleep(settings.StubExitTimeSpan * 2);
		    Assert.That(settings.StubExitProcessService.ExitCode, Is.EqualTo(65));

		    finish.Set();
	    }
    }
}