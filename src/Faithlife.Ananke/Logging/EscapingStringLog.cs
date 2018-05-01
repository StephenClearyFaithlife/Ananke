using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Logging
{
	/// <summary>
	/// A string logger that backslash-escapes EOL characters before passing them to an inner logger.
	/// </summary>
    public sealed class EscapingStringLog : IStringLog
    {
		/// <summary>
		/// Creates a new escaping log wrapper around an existing log.
		/// </summary>
		/// <param name="log">The inner logger.</param>
	    public EscapingStringLog(IStringLog log)
	    {
		    m_log = log;
	    }

		/// <inheritdoc />
	    public void WriteLine(string message) => m_log.WriteLine(message.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r"));

	    private readonly IStringLog m_log;
	}
}
