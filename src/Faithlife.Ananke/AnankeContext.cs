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
		/// <param name="exitRequested">The cancellation token which is cancelled when the application is requestd to exit.</param>
		/// <param name="escapingConsoleStdout">The escaping text writer.</param>
		/// <param name="loggingConsoleStdout">The logging text writer.</param>
		public AnankeContext(ILoggerProvider loggerProvider, CancellationToken exitRequested, TextWriter escapingConsoleStdout, TextWriter loggingConsoleStdout)
		{
			LoggerProvider = loggerProvider;
			ExitRequested = exitRequested;
			EscapingConsoleStdout = escapingConsoleStdout;
			LoggingConsoleStdout = loggingConsoleStdout;
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
		/// A text writer that writes to <see cref="AnankeSettings.ConsoleLog"/> after backslash-escaping EOL characters. You must explicitly request an EOL by calling one of the <c>WriteLine</c> methods.
		/// </summary>
		public TextWriter EscapingConsoleStdout { get; }

		/// <summary>
		/// A text writer that writes to <see cref="LoggerProvider"/>.
		/// </summary>
		public TextWriter LoggingConsoleStdout { get; }
    }
}
