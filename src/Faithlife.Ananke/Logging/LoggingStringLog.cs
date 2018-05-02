using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke.Logging
{
	/// <summary>
	/// A string log that writes to an <see cref="ILogger"/>.
	/// </summary>
    public sealed class LoggingStringLog : IStringLog
    {
	    /// <summary>
	    /// Creates a string log that writes to loggers provided by the given logger provider.
	    /// </summary>
	    /// <param name="loggerProvider">The logger provider.</param>
	    /// <param name="stdoutParser">The parser to determine how to log messages.</param>
	    public LoggingStringLog(ILoggerProvider loggerProvider, AnankeSettings.StdoutParserDelegate stdoutParser)
	    {
		    m_loggerProvider = loggerProvider;
		    m_stdoutParser = stdoutParser;
	    }

		/// <inheritdoc/>
	    public void WriteLine(string message) => m_stdoutParser(message, m_loggerProvider);

	    private readonly ILoggerProvider m_loggerProvider;
	    private readonly AnankeSettings.StdoutParserDelegate m_stdoutParser;
    }
}
