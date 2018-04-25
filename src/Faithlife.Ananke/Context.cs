using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
	    public Context(IStringLogService stringLog, TextWriter escapingTextWriter)
		{
			StringLog = stringLog;
			m_escapingTextWriter = escapingTextWriter;
		}

		/// <summary>
		/// Service that writes strings to a log. You may send messages that contain EOL characters to this instance; they will be escaped before they are passed to <see cref="Settings.ConsoleLogService"/>.
		/// </summary>
		public IStringLogService StringLog { get; }

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
