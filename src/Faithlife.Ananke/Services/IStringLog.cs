using System;
using System.Collections.Generic;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <summary>
	/// Service that writes strings to a log.
	/// </summary>
	public interface IStringLog
	{
		/// <summary>
		/// Writes a single message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		void WriteLine(string message);
	}
}
