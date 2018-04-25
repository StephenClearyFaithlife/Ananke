using System;
using System.IO;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// Settings used to control Ananke behavior.
	/// </summary>
	public sealed class Settings
    {
	    private Settings(IStringLogService consoleLogService, IExitProcessService exitProcessService, ISignalService sigintSignalService, ISignalService sigtermSignalService)
	    {
			ConsoleLogService = consoleLogService;
			ExitProcessService = exitProcessService;
		    SigintSignalService = sigintSignalService;
		    SigtermSignalService = sigtermSignalService;
	    }

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
		/// Creates an instance of <see cref="Settings"/>, with default settings for any setting not specified.
		/// </summary>
		/// <param name="consoleLogService">Service that writes strings to the console. This is wrapped with a formatting text writer to escape EOL characters.</param>
		/// <param name="exitProcessService">Service that exits the entire process.</param>
		/// <param name="sigintSignalService">Service that hooks SIGINT.</param>
		/// <param name="sigtermSignalService">Service that hooks SIGTERM.</param>
		public static Settings Create(IStringLogService consoleLogService = null, IExitProcessService exitProcessService = null,
			ISignalService sigintSignalService = null, ISignalService sigtermSignalService = null)
		{
			return new Settings(consoleLogService ?? new TextWriterStringLogService(Console.Out),
				exitProcessService ?? new ExitProcessService(),
				sigintSignalService ?? new SigintSignalService(),
				sigtermSignalService ?? new SigtermSignalService());
		}
	}
}
