using System;
using System.Collections.Generic;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <summary>
	/// Service that exits the entire process.
	/// </summary>
	public interface IExitProcessService
	{
		/// <summary>
		/// Exit the entire process. This method may or may not return. If it returns, it will return the exit code.
		/// </summary>
		/// <param name="exitCode">The exit code to pass to the operating system.</param>
		int Exit(int exitCode);
	}

	/// <inheritdoc/>
    public sealed class ExitProcessService: IExitProcessService
	{
		/// <inheritdoc/>
	    public int Exit(int exitCode)
	    {
		    Environment.Exit(exitCode);
		    return exitCode;
	    }
    }
}
