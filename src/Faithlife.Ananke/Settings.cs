﻿using System;
using System.IO;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// Settings used to control Ananke behavior.
	/// </summary>
	public sealed class Settings
    {
	    private Settings(IStringLogService stringLogService, IExitProcessService exitProcessService)
	    {
			StringLogService = stringLogService;
			ExitProcessService = exitProcessService;
	    }

		/// <summary>
		/// Service that writes strings to a log.
		/// </summary>
		public IStringLogService StringLogService { get; }

		/// <summary>
		/// Service that exits the entire process.
		/// </summary>
		public IExitProcessService ExitProcessService { get; }

		/// <summary>
		/// Creates an instance of <see cref="Settings"/>, with default settings for any setting not specified.
		/// </summary>
		/// <param name="consoleStringLogService">Service that writes strings to the console. This is wrapped with a formatting text writer to escape EOL characters.</param>
		/// <param name="exitProcessService">Service that exits the entire process.</param>
		public static Settings Create(IStringLogService consoleStringLogService = null,
			IExitProcessService exitProcessService = null)
		{
			consoleStringLogService = consoleStringLogService ?? new TextWriterStringLogService(Console.Out);
			return new Settings(new TextWriterStringLogService(new EscapingStringLogTextWriter(consoleStringLogService)), 
				exitProcessService ?? new ExitProcessService());
		}
	}
}
