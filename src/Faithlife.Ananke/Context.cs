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
		public Context(IStringLogService stringLog)
		{
			StringLog = stringLog;
		}

		/// <summary>
		/// Service that writes strings to a log. You may send messages that contain EOL characters to this instance; they will be escaped before they are passed to <see cref="Settings.ConsoleLogService"/>.
		/// </summary>
		public IStringLogService StringLog { get; }
    }
}
