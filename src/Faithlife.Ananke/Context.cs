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
	    /// <param name="escapingTextWriter">An escaping text writer.</param>
	    /// <param name="exitRequested">A cancellation token which is cancelled when the application is requestd to exit.</param>
	    public Context(IStringLogService stringLog, TextWriter escapingTextWriter, CancellationToken exitRequested)
		{
			StringLog = stringLog;
			m_escapingTextWriter = escapingTextWriter;
			ExitRequested = exitRequested;
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
		/// Intercepts writes to <see cref="Console.Out"/> and <see cref="Console.Error"/> with an EOL-escaping writer. This requires application code to use <c>WriteLine</c> in order to see an actual EOL.
		/// </summary>
		public void InterceptConsoleOutputs()
	    {
			Console.WriteLine("Test");
			Console.SetOut(m_escapingTextWriter);
			Console.SetError(m_escapingTextWriter);
	    }

		private readonly TextWriter m_escapingTextWriter;
    }
}
