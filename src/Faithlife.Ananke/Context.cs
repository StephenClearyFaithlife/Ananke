using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// The execution context for the application.
	/// </summary>
    public sealed class Context
    {
		/// <summary>
		/// Creates an execution context for application code.
		/// </summary>
		/// <param name="stringLog">The string log service. This service must escape EOL characters.</param>
		/// <param name="exitRequested">A cancellation token which is cancelled when the application is requestd to exit.</param>
		/// <param name="escapedConsoleStdout">An escaping text writer for stdout.</param>
		/// <param name="escapedConsoleStderr">An escaping text writer for stderr.</param>
		public Context(IStringLogService stringLog, CancellationToken exitRequested, TextWriter escapedConsoleStdout, TextWriter escapedConsoleStderr)
		{
			StringLog = stringLog;
			ExitRequested = exitRequested;
			EscapedConsoleStdout = escapedConsoleStdout;
			EscapedConsoleStderr = escapedConsoleStderr;
		}

		/// <summary>
		/// Service that writes strings to a log. You may send messages that contain EOL characters to this instance; they will be escaped before they are passed to <see cref="Settings.ConsoleLogService"/>.
		/// </summary>
		public IStringLogService StringLog { get; }

	    /// <summary>
	    /// The application has been requested to exit.
	    /// </summary>
	    public CancellationToken ExitRequested { get; }

		/// <summary>
		/// A text writer that writes to <see cref="Console.Out"/> after backslash-escaping EOL characters. You must explicitly request an EOL by calling one of the <c>WriteLine</c> methods.
		/// </summary>
		public TextWriter EscapedConsoleStdout { get; }

	    /// <summary>
	    /// A text writer that writes to <see cref="Console.Error"/> after backslash-escaping EOL characters. You must explicitly request an EOL by calling one of the <c>WriteLine</c> methods.
	    /// </summary>
	    public TextWriter EscapedConsoleStderr { get; }
    }
}
