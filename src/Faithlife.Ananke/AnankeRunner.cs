using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Ananke.Logging;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// The Ananke wrapper around your code. Normally, you should call the static <see cref="O:Faithlife.Ananke.AnankeRunner.Main"/> method rather than create instances of this type directly.
	/// </summary>
    public sealed class AnankeRunner
    {
		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(AnankeSettings settings, Func<AnankeContext, Task<int>> action)
	    {
		    var runner = new AnankeRunner(settings);
		    return runner.Run(action);
	    }

		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(AnankeSettings settings, Func<AnankeContext, Task> action) => Main(settings, async context =>
	    {
		    await action(context).ConfigureAwait(false);
		    return c_successExitCode;
	    });

#pragma warning disable 1998
		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(AnankeSettings settings, Func<AnankeContext, int> action) => Main(settings, async context => action(context));
#pragma warning restore

		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(AnankeSettings settings, Action<AnankeContext> action) => Main(settings, context =>
	    {
		    action(context);
		    return c_successExitCode;
	    });

	    /// <summary>
	    /// Creates an Ananke wrapper with the specified settings.
	    /// </summary>
	    /// <param name="settings">The settings to use.</param>
	    private AnankeRunner(AnankeSettings settings)
	    {
		    m_settings = settings;
		    m_log = new EscapingStringLog(m_settings.ConsoleLogService);
		    m_exitRequested = new CancellationTokenSource();
		    m_context = new AnankeContext(m_log, m_exitRequested.Token, new EscapingTextWriter(m_settings.ConsoleStdout), new EscapingTextWriter(m_settings.ConsoleStderr));
		    m_done = new ManualResetEventSlim();
	    }

	    /// <summary>
	    /// Executes application logic within this wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
	    /// </summary>
	    /// <param name="action">The application logic to execute.</param>
	    private int Run(Func<AnankeContext, Task<int>> action)
	    {
		    try
		    {
			    // Hook SIGINT and SIGTERM
			    m_settings.SigintSignalService.AddHandler(() => Shutdown("SIGINT received."));
			    m_settings.SigtermSignalService.AddHandler(() =>
			    {
				    Shutdown("SIGTERM received.");
				    m_done.Wait();
			    });

				// Exit after our maximum runtime.
				ShutdownAfterMaximumRuntime();

			    var exitCode = action(m_context).GetAwaiter().GetResult();
			    return m_settings.ExitProcessService.Exit(exitCode);
		    }
		    catch (OperationCanceledException) when (m_exitRequested.IsCancellationRequested)
		    {
			    m_log.WriteLine("Ignoring OperationCanceledException since we are shutting down.");
			    return m_settings.ExitProcessService.Exit(c_successExitCode);
		    }
		    catch (Exception ex)
		    {
			    m_log.WriteLine(ex.ToString());
			    return m_settings.ExitProcessService.Exit(c_unexpectedExceptionExitCode);
		    }
		    finally
		    {
			    m_done.Set();
		    }
	    }

		/// <summary>
		/// Initiates an automatic shutdown after the application has been running for its maximum runtime.
		/// </summary>
	    private async void ShutdownAfterMaximumRuntime()
	    {
		    await Task.Delay(m_settings.MaximumRuntime);
		    Shutdown($"Maximum runtime of {m_settings.MaximumRuntime} reached.");
	    }

		/// <summary>
		/// Initiates a shutdown. Sets <see cref="AnankeContext.ExitRequested"/> and starts the exit timeout.
		/// </summary>
		/// <param name="reason">The reason for the shutdown, written to the console log.</param>
		private void Shutdown(string reason)
	    {
		    m_log.WriteLine("Shutting down: " + reason);
			ExitAfterTimeout();
		    m_exitRequested.Cancel();
	    }

		/// <summary>
		/// Waits for <see cref="AnankeSettings.ExitTimeout"/> and then sets <see cref="m_done"/> and calls <see cref="IExitProcessService.Exit"/>.
		/// This method is always asynchronous.
		/// </summary>
	    private async void ExitAfterTimeout()
	    {
			// If ExitTimeout is zero, we want to force asynchrony so that ExitAfterTimeout returns before calling ExitPocessService.Exit.
			await Task.Yield();

			await Task.Delay(m_settings.ExitTimeout);
			m_done.Set();
			m_settings.ExitProcessService.Exit(c_exitTimeoutExitCode);
	    }

		private readonly AnankeSettings m_settings;
		private readonly IStringLogService m_log;
		private readonly AnankeContext m_context;
	    private readonly CancellationTokenSource m_exitRequested;
	    private readonly ManualResetEventSlim m_done;

	    private const int c_successExitCode = 0;
		private const int c_unexpectedExceptionExitCode = 64;
	    private const int c_exitTimeoutExitCode = 65;
    }
}
