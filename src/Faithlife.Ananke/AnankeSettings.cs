using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Faithlife.Ananke.Logging;
using Faithlife.Ananke.Services;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke
{
	/// <summary>
	/// Settings used to control Ananke behavior.
	/// </summary>
	public sealed class AnankeSettings
	{
		/// <summary>
		/// A delegate that parses <paramref name="message"/> and logs it to <paramref name="loggerProvider"/>.
		/// </summary>
		/// <param name="message">The message; this is a text that has been written to the console.</param>
		/// <param name="loggerProvider">The logger provider to log to.</param>
		public delegate void StdoutParserDelegate(string message, ILoggerProvider loggerProvider);

		/// <summary>
		/// The maximum amount of time the application will run.
		/// </summary>
		public TimeSpan MaximumRuntime { get; }

		/// <summary>
		/// The core logging provider used by all structured logging.
		/// </summary>
		public ILoggerProvider LoggerProvider { get; }

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
		/// A method that parses text written to stdout.
		/// </summary>
		public StdoutParserDelegate StdoutParser { get; }

		/// <summary>
		/// Service that writes strings to the console. The strings passed to this log will not contain EOL characters.
		/// </summary>
		internal IStringLog ConsoleLog { get; }

		/// <summary>
		/// Service that exits the entire process.
		/// </summary>
		internal IExitProcessService ExitProcessService { get; }

		/// <summary>
		/// Service that hooks shutdown signals sent to the process.
		/// </summary>
		internal ISignalService SignalService { get; }

		/// <summary>
		/// Creates an instance of <see cref="AnankeSettings"/>, with default settings for any setting not specified.
		/// </summary>
		/// <param name="maximumRuntime">The amonut of time application code should run until it is requested to exit. Defaults to infinite, but most apps should use a non-infinite time.</param>
		/// <param name="loggerProvider">The core logging provider used by all structured logging. Defaults to a logging provider that writes formatted text to <see cref="ConsoleLog"/>.</param>
		/// <param name="loggerIsEnabledFilter">The filter used by the <see cref="AnankeLoggerProvider"/> if <paramref name="loggerProvider"/> is <c>null</c>.</param>
		/// <param name="loggerFormatter">The formatter used by the <see cref="AnankeLoggerProvider"/> if <paramref name="loggerProvider"/> is <c>null</c>.</param>
		/// <param name="exitTimeout">The amonut of time application code has after it is requested to exit, before the process forcibly exits. Defaults to 10 seconds.</param>
		/// <param name="randomMaximumRuntimeRelativeDelta">The amount of random fluction in <see cref="MaximumRuntime"/>. E.g., <c>0.10</c> is a 10% change; if <see cref="MaximumRuntime"/> is 30 minutes, then the actual maximum runtime would be a random value between 27 and 33 minutes. Defaults to 0.10 (10%).</param>
		/// <param name="stdoutParser">A method that parses text written to stdout.</param>
		public static AnankeSettings Create(TimeSpan? maximumRuntime = null, ILoggerProvider loggerProvider = null,
			LoggerIsEnabledFilter loggerIsEnabledFilter = null, LoggerFormatter loggerFormatter = null,
			TimeSpan? exitTimeout = null, double? randomMaximumRuntimeRelativeDelta = null, StdoutParserDelegate stdoutParser = null)
		{
			return InternalCreate(maximumRuntime, loggerProvider, loggerIsEnabledFilter, loggerFormatter,
				exitTimeout, randomMaximumRuntimeRelativeDelta, stdoutParser);
		}

		internal static AnankeSettings InternalCreate(TimeSpan? maximumRuntime = null, ILoggerProvider loggerProvider = null,
			LoggerIsEnabledFilter loggerIsEnabledFilter = null, LoggerFormatter loggerFormatter = null,
			TimeSpan? exitTimeout = null, double? randomMaximumRuntimeRelativeDelta = null, StdoutParserDelegate stdoutParser = null, IStringLog consoleLog = null,
			IExitProcessService exitProcessService = null, ISignalService signalService = null)
		{
			if (signalService == null)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					signalService = new WindowsSignalService();
				else
					signalService = new UnixSignalService();
			}

			consoleLog = consoleLog ?? new TextWriterStringLog(Console.Out);
			loggerFormatter = loggerFormatter ?? AnankeFormatters.FormattedText;
			loggerIsEnabledFilter = loggerIsEnabledFilter ?? ((_, __) => true);
			return new AnankeSettings(maximumRuntime ?? Timeout.InfiniteTimeSpan,
				loggerProvider ?? new AnankeLoggerProvider(consoleLog, loggerFormatter, loggerIsEnabledFilter),
				exitTimeout ?? TimeSpan.FromSeconds(10),
				randomMaximumRuntimeRelativeDelta ?? 0.10,
				stdoutParser ?? ((message, provider) => provider.CreateLogger("App").LogInformation(message)),
				consoleLog,
				exitProcessService ?? new ExitProcessService(),
				signalService);
		}

		private AnankeSettings(TimeSpan maximumRuntime, ILoggerProvider loggerProvider, TimeSpan exitTimeout, double randomMaximumRuntimeRelativeDelta,
			StdoutParserDelegate stdoutParser, IStringLog consoleLog, IExitProcessService exitProcessService, ISignalService signalService)
	    {
			MaximumRuntime = maximumRuntime;
		    LoggerProvider = loggerProvider;
			ExitTimeout = exitTimeout;
		    RandomMaximumRuntimeRelativeDelta = randomMaximumRuntimeRelativeDelta;
		    StdoutParser = stdoutParser;
		    ConsoleLog = consoleLog;
		    ExitProcessService = exitProcessService;
		    SignalService = signalService;
	    }
    }
}
