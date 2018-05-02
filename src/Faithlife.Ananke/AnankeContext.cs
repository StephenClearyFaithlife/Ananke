using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke
{
	/// <summary>
	/// The execution context for the application.
	/// </summary>
    public sealed class AnankeContext
    {
	    /// <summary>
	    /// Creates an execution context for application code.
	    /// </summary>
	    /// <param name="loggerProvider">The logger provider.</param>
	    /// <param name="exitRequested">A cancellation token which is cancelled when the application is requestd to exit.</param>
	    /// <param name="escapedConsoleStdout">An escaping text writer that writes to stdout.</param>
	    public AnankeContext(ILoggerProvider loggerProvider, CancellationToken exitRequested, TextWriter escapedConsoleStdout)
		{
			LoggerProvider = loggerProvider;
			ExitRequested = exitRequested;
			EscapedConsoleStdout = escapedConsoleStdout;
		}

		/// <summary>
		/// Logger provider that writes formatted strings to <see cref="AnankeSettings.ConsoleLog"/>.
		/// </summary>
		public ILoggerProvider LoggerProvider { get; }

	    /// <summary>
	    /// The application has been requested to exit.
	    /// </summary>
	    public CancellationToken ExitRequested { get; }

		/// <summary>
		/// A text writer that writes to <see cref="Console.Out"/> after backslash-escaping EOL characters. You must explicitly request an EOL by calling one of the <c>WriteLine</c> methods.
		/// </summary>
		public TextWriter EscapedConsoleStdout { get; }
    }
}
