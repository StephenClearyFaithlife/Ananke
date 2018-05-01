using System;
using System.IO;
using System.Runtime.InteropServices;
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
		public IStringLog ConsoleLog { get; }

		/// <summary>
		/// Service that exits the entire process.
		/// </summary>
		public IExitProcessService ExitProcessService { get; }

	    /// <summary>
	    /// Service that hooks shutdown signals sent to the process.
	    /// </summary>
	    public ISignalService SignalService { get; }

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
		/// <param name="randomMaximumRuntimeRelativeDelta">The amount of random fluction in <see cref="MaximumRuntime"/>. E.g., <c>0.10</c> is a 10% change; if <see cref="MaximumRuntime"/> is 30 minutes, then the actual maximum runtime would be a random value between 27 and 33 minutes. Defaults to 0.10 (10%).</param>
		/// <param name="consoleLog">Service that writes strings to the console. This is wrapped with a formatting text writer to escape EOL characters.</param>
		/// <param name="exitProcessService">Service that exits the entire process.</param>
		/// <param name="signalService">Service that hooks shutdown signals sent to the process.</param>
		/// <param name="consoleStdout">The standard output stream.</param>
		/// <param name="consoleStderr">The standard error stream.</param>
		public static AnankeSettings Create(TimeSpan? maximumRuntime = null, TimeSpan? exitTimeout = null, double? randomMaximumRuntimeRelativeDelta = null,
			IStringLog consoleLog = null, IExitProcessService exitProcessService = null, ISignalService signalService = null,
			TextWriter consoleStdout = null, TextWriter consoleStderr = null)
		{
			consoleStdout = consoleStdout ?? Console.Out;
			if (signalService == null)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					signalService = new WindowsSignalService();
				else
					signalService = new UnixSignalService();
			}
			return new AnankeSettings(maximumRuntime ?? Timeout.InfiniteTimeSpan,
				exitTimeout ?? TimeSpan.FromSeconds(10),
				randomMaximumRuntimeRelativeDelta ?? 0.10,
				consoleLog ?? new TextWriterStringLog(consoleStdout),
				exitProcessService ?? new ExitProcessService(),
				signalService,
				consoleStdout,
				consoleStderr ?? Console.Error);
		}

	    private AnankeSettings(TimeSpan maximumRuntime, TimeSpan exitTimeout, double randomMaximumRuntimeRelativeDelta, IStringLog consoleLog,
		    IExitProcessService exitProcessService, ISignalService signalService, TextWriter consoleStdout, TextWriter consoleStderr)
	    {
			MaximumRuntime = maximumRuntime;
		    ExitTimeout = exitTimeout;
		    RandomMaximumRuntimeRelativeDelta = randomMaximumRuntimeRelativeDelta;
		    ConsoleLog = consoleLog;
		    ExitProcessService = exitProcessService;
		    SignalService = signalService;
		    ConsoleStdout = consoleStdout;
		    ConsoleStderr = consoleStderr;
	    }
    }
}
