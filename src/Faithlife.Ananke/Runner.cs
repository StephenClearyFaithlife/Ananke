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
	/// The Ananke wrapper around your code. Normally, you should call the static <see cref="O:Faithlife.Ananke.Runner.Main"/> method rather than create instances of this type directly.
	/// </summary>
    public sealed class Runner
    {
		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(Settings settings, Func<Context, Task<int>> action)
	    {
		    var runner = new Runner(settings);
		    return runner.Run(action);
	    }

		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(Settings settings, Func<Context, Task> action) => Main(settings, async context =>
	    {
		    await action(context).ConfigureAwait(false);
		    return 0;
	    });

#pragma warning disable 1998
		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(Settings settings, Func<Context, int> action) => Main(settings, async context => action(context));
#pragma warning restore

		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(Settings settings, Action<Context> action) => Main(settings, context =>
	    {
		    action(context);
		    return 0;
	    });

	    /// <summary>
	    /// Creates an Ananke wrapper with the specified settings.
	    /// </summary>
	    /// <param name="settings">The settings to use.</param>
	    private Runner(Settings settings)
	    {
		    m_settings = settings;
		    m_log = new EscapingStringLog(m_settings.ConsoleLogService);
		    m_exitRequested = new CancellationTokenSource();
		    m_context = new Context(m_log, m_exitRequested.Token, new EscapingTextWriter(m_settings.ConsoleStdout), new EscapingTextWriter(m_settings.ConsoleStderr));
		    m_done = new ManualResetEventSlim();
	    }

	    /// <summary>
	    /// Executes application logic within this wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
	    /// </summary>
	    /// <param name="action">The application logic to execute.</param>
	    private int Run(Func<Context, Task<int>> action)
	    {
		    try
		    {
			    // Hook SIGINT and SIGTERM
			    m_settings.SigintSignalService.AddHandler(() => Shutdown("SIGINT received"));
			    m_settings.SigtermSignalService.AddHandler(() =>
			    {
				    Shutdown("SIGTERM received");
				    m_done.Wait();
			    });

			    var exitCode = action(m_context).GetAwaiter().GetResult();
			    return m_settings.ExitProcessService.Exit(exitCode);
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
	    private void Shutdown(string reason)
	    {
		    m_log.WriteLine("Shutting down: " + reason);
		    m_exitRequested.Cancel();
	    }

		private readonly Settings m_settings;
		private readonly IStringLogService m_log;
		private readonly Context m_context;
	    private readonly CancellationTokenSource m_exitRequested;
	    private readonly ManualResetEventSlim m_done;

		private const int c_unexpectedExceptionExitCode = 64;
    }
}
