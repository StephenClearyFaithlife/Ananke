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
		/// <param name="loggingConsoleStdout">The logging text writer.</param>
		public AnankeContext(ILoggerProvider loggerProvider, CancellationToken exitRequested, TextWriter loggingConsoleStdout)
		{
			LoggerProvider = loggerProvider;
			ExitRequested = exitRequested;
			LoggingConsoleStdout = loggingConsoleStdout;
		}

		/// <summary>
		/// Same as <see cref="AnankeSettings.LoggerProvider"/>.
		/// </summary>
		public ILoggerProvider LoggerProvider { get; }

	    /// <summary>
	    /// The application has been requested to exit.
	    /// </summary>
	    public CancellationToken ExitRequested { get; }

		/// <summary>
		/// A text writer that writes to <see cref="LoggerProvider"/>. You must explicitly request a log event by calling one of the <c>WriteLine</c> methods.
		/// </summary>
		public TextWriter LoggingConsoleStdout { get; }
    }
}
