using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// The Ananke wrapper around your code. Normally, you should call the static <see cref="O:Faithlife.Ananke.Runner.Main"/> method rather than create instances of this type directly.
	/// </summary>
    public sealed class Runner
    {
		/// <summary>
		/// Creates an Ananke wrapper with the specified settings. Normally, you should call the static <see cref="O:Faithlife.Ananke.Runner.Main"/> method rather than create instances of this type directly.
		/// </summary>
		/// <param name="settings">The settings to use.</param>
		public Runner(Settings settings)
	    {
		    m_settings = settings;
		    m_log = new TextWriterStringLogService(new EscapingStringLogTextWriter(m_settings.ConsoleLogService));
			m_context = new Context(m_log);
	    }

		/// <summary>
		/// Executes application logic within this wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="action">The application logic to execute.</param>
		public async Task<int> Run(Func<Context, Task<int>> action)
	    {
		    try
		    {
			    var result = await action(m_context).ConfigureAwait(false);
			    return m_settings.ExitProcessService.Exit(result);
		    }
			catch (Exception ex)
		    {
				m_log.WriteLine(ex.ToString());
			    return m_settings.ExitProcessService.Exit(c_unexpectedExceptionExitCode);
		    }
	    }

	    /// <summary>
	    /// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
	    /// </summary>
	    /// <param name="settings">The settings to use for the Ananke wrapper.</param>
	    /// <param name="action">The application logic to execute.</param>
	    public static Task<int> Main(Settings settings, Func<Context, Task<int>> action)
	    {
		    var runner = new Runner(settings);
		    return runner.Run(action);
	    }

	    /// <summary>
	    /// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
	    /// </summary>
	    /// <param name="settings">The settings to use for the Ananke wrapper.</param>
	    /// <param name="action">The application logic to execute.</param>
	    public static Task<int> Main(Settings settings, Func<Context, Task> action) => Main(settings, async context =>
	    {
		    await action(context).ConfigureAwait(false);
		    return 0;
	    });

		/// <summary>
		/// Creates an Ananke wrapper and executes the application logic within that wrapper. This method only returns if <see cref="IExitProcessService.Exit"/> returns.
		/// </summary>
		/// <param name="settings">The settings to use for the Ananke wrapper.</param>
		/// <param name="action">The application logic to execute.</param>
		public static int Main(Settings settings, Func<Context, int> action) => Main(settings, context => Task.FromResult(action(context))).GetAwaiter().GetResult();

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

	    private readonly Settings m_settings;
		private readonly IStringLogService m_log;
		private readonly Context m_context;

	    private const int c_unexpectedExceptionExitCode = 64;
    }
}
