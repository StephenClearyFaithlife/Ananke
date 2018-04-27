using System;
using System.IO;
using System.Threading;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// Settings used to control Ananke behavior.
	/// </summary>
	public sealed class AnankeSettings
    {
		/// <summary>
		/// The maximum amount of time the application will run.
		/// </summary>
		public TimeSpan MaximumRuntime { get; }

		/// <summary>
		/// The amonut of time application code has after it is requested to exit, before the process forcibly exits.
		/// </summary>
		public TimeSpan ExitTimeout { get; }
	    
		/// <summary>
		/// The amount of random fluction in <see cref="MaximumRuntime"/>.
		/// E.g., <c>0.10</c> is a 10% change; if <see cref="MaximumRuntime"/> is 30 minutes, then the actual maximum runtime would be a random value between 27 and 33 minutes.
		/// </summary>
		public double RandomMaximumRuntimeRelativeDelta { get; }

	    /// <summary>
		/// Service that writes strings to the console. The strings passed to this log will not contain EOL characters.
		/// </summary>
		public IStringLogService ConsoleLogService { get; }

		/// <summary>
		/// Service that exits the entire process.
		/// </summary>
		public IExitProcessService ExitProcessService { get; }

	    /// <summary>
	    /// Service that hooks SIGINT.
	    /// </summary>
	    public ISignalService SigintSignalService { get; }

	    /// <summary>
	    /// Service that hooks SIGTERM.
	    /// </summary>
	    public ISignalService SigtermSignalService { get; }

		/// <summary>
		/// The standard output stream.
		/// </summary>
		public TextWriter ConsoleStdout { get; }

		/// <summary>
		/// The standard error stream.
		/// </summary>
		public TextWriter ConsoleStderr { get; }

		/// <summary>
		/// Creates an instance of <see cref="AnankeSettings"/>, with default settings for any setting not specified.
		/// </summary>
		/// <param name="maximumRuntime">The amonut of time application code should run until it is requested to exit. Defaults to infinite, but most apps should use a non-infinite time.</param>
		/// <param name="exitTimeout">The amonut of time application code has after it is requested to exit, before the process forcibly exits. Defaults to 10 seconds.</param>
		/// <param name="randomMaximumRuntimeRelativeDelta">The amount of random fluction in <see cref="MaximumRuntime"/>. E.g., <c>0.10</c> is a 10% change; if <see cref="MaximumRuntime"/> is 30 minutes, then the actual maximum runtime would be a random value between 27 and 33 minutes. Defaults to 0.20 (20%).</param>
		/// <param name="consoleLogService">Service that writes strings to the console. This is wrapped with a formatting text writer to escape EOL characters.</param>
		/// <param name="exitProcessService">Service that exits the entire process.</param>
		/// <param name="sigintSignalService">Service that hooks SIGINT.</param>
		/// <param name="sigtermSignalService">Service that hooks SIGTERM.</param>
		/// <param name="consoleStdout">The standard output stream.</param>
		/// <param name="consoleStderr">The standard error stream.</param>
		public static AnankeSettings Create(TimeSpan? maximumRuntime = null, TimeSpan? exitTimeout = null, double? randomMaximumRuntimeRelativeDelta = null,
			IStringLogService consoleLogService = null, IExitProcessService exitProcessService = null, ISignalService sigintSignalService = null,
			ISignalService sigtermSignalService = null, TextWriter consoleStdout = null, TextWriter consoleStderr = null)
		{
			consoleStdout = consoleStdout ?? Console.Out;
			return new AnankeSettings(maximumRuntime ?? Timeout.InfiniteTimeSpan,
				exitTimeout ?? TimeSpan.FromSeconds(10),
				randomMaximumRuntimeRelativeDelta ?? 0.20,
				consoleLogService ?? new TextWriterStringLogService(consoleStdout),
				exitProcessService ?? new ExitProcessService(),
				sigintSignalService ?? new SigintSignalService(),
				sigtermSignalService ?? new SigtermSignalService(),
				consoleStdout,
				consoleStderr ?? Console.Error);
		}

	    private AnankeSettings(TimeSpan maximumRuntime, TimeSpan exitTimeout, double randomMaximumRuntimeRelativeDelta, IStringLogService consoleLogService,
		    IExitProcessService exitProcessService, ISignalService sigintSignalService, ISignalService sigtermSignalService, TextWriter consoleStdout, TextWriter consoleStderr)
	    {
			MaximumRuntime = maximumRuntime;
		    ExitTimeout = exitTimeout;
		    RandomMaximumRuntimeRelativeDelta = randomMaximumRuntimeRelativeDelta;
		    ConsoleLogService = consoleLogService;
		    ExitProcessService = exitProcessService;
		    SigintSignalService = sigintSignalService;
		    SigtermSignalService = sigtermSignalService;
		    ConsoleStdout = consoleStdout;
		    ConsoleStderr = consoleStderr;
	    }
    }
}
