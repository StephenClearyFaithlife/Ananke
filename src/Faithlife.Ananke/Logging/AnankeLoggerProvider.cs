using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke.Logging
{
	/// <inheritdoc />
	/// <summary>
	/// The logger provider used by Ananke. Sends logs to an <see cref="IStringLog"/>.
	/// </summary>
	public sealed partial class AnankeLoggerProvider : ILoggerProvider
    {
	    /// <summary>
	    /// Creates a new logger provider.
	    /// </summary>
	    /// <param name="stringLog">The underlying string log to which all logs are written.</param>
	    /// <param name="formatter">The formatter used to translate log events into single-line strings.</param>
	    public AnankeLoggerProvider(IStringLog stringLog, Formatter formatter)
	    {
		    m_stringLog = stringLog;
		    m_formatter = formatter;
			m_loggers = new ConcurrentDictionary<string, ILogger>();
	    }

		/// <summary>
		/// Formats a log event into a single-line string. The resulting string must not have any EOL characters.
		/// </summary>
		/// <param name="loggerName">The name (category) of the logger. May not be <c>null</c>.</param>
		/// <param name="logLevel">The importance of the event.</param>
		/// <param name="eventId">The id of the event, or <c>0</c> if there is no id.</param>
		/// <param name="message">The message. May not be <c>null</c>, but may be the empty string.</param>
		/// <param name="exception">The exception, if any. May be <c>null</c>.</param>
		public delegate string Formatter(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception); // TODO: add scope parameter

	    /// <inheritdoc/>
	    public ILogger CreateLogger(string categoryName)
	    {
			if (categoryName == null)
				throw new ArgumentNullException(nameof(categoryName));
			return m_loggers.GetOrAdd(categoryName, name => new AnankeLogger(this, name));
		}

	    void IDisposable.Dispose() { }

	    private void Log(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception)
	    {
		    if (message == "" && exception == null)
			    return;
			m_stringLog.WriteLine(m_formatter(loggerName, logLevel, eventId, message, exception));
	    }

	    private bool IsEnabled(string name, LogLevel logLevel)
	    {
		    // TODO: allow filtering
		    return true;
	    }

	    private IDisposable BeginScope<TState>(TState state)
	    {
			// TODO: handle scopes
		    return null;
	    }

	    private readonly IStringLog m_stringLog;
	    private readonly Formatter m_formatter;
	    private readonly ConcurrentDictionary<string, ILogger> m_loggers;
    }
}
