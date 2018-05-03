using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
		/// <param name="stringLog">The underlying string log to which all logs are written. May not be <c>null</c>.</param>
		/// <param name="formatter">The formatter used to translate log events into single-line strings. May not be <c>null</c>.</param>
		/// <param name="filter">The filter for determining which log events to log. May not be <c>null</c>.</param>
		public AnankeLoggerProvider(IStringLog stringLog, Formatter formatter, IsEnabledFilter filter)
	    {
			if (stringLog == null)
				throw new ArgumentNullException(nameof(stringLog));
		    if (formatter == null)
			    throw new ArgumentNullException(nameof(formatter));
		    if (filter == null)
			    throw new ArgumentNullException(nameof(filter));

			m_stringLog = stringLog;
		    m_formatter = formatter;
		    m_filter = filter;
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
		/// <param name="state">The structured state for the message, if any. May be <c>null</c>.</param>
		/// <param name="scope">The structured scope for the message, if any. May be an empty sequence.</param>
		/// <param name="scopeMessages">The scope for the message (as strings), if any. May be an empty sequence.</param>
		public delegate string Formatter(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception,
			IEnumerable<KeyValuePair<string, object>> state, IEnumerable<IEnumerable<KeyValuePair<string, object>>> scope, IEnumerable<string> scopeMessages);

		/// <summary>
		/// Determines whether a log event is enabled. Any events for which this method returns <c>false</c> are not logged.
		/// </summary>
		/// <param name="loggerName">The name (category) of the logger. May not be <c>null</c>.</param>
		/// <param name="logLevel">The importance of the event.</param>
	    public delegate bool IsEnabledFilter(string loggerName, LogLevel logLevel);

	    /// <inheritdoc/>
	    public ILogger CreateLogger(string categoryName)
	    {
			if (categoryName == null)
				throw new ArgumentNullException(nameof(categoryName));
			return m_loggers.GetOrAdd(categoryName, name => new AnankeLogger(this, name));
		}

	    void IDisposable.Dispose() { }

	    private void Log(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception, IEnumerable<KeyValuePair<string, object>> state)
	    {
		    if (message == "" && exception == null)
			    return;
			// TODO: collect scope information and pass along
		    var text = m_formatter(loggerName, logLevel, eventId, message, exception, state,
			    Enumerable.Empty<IEnumerable<KeyValuePair<string, object>>>(),
			    Enumerable.Empty<string>());
			m_stringLog.WriteLine(text);
	    }

	    private bool IsEnabled(string loggerName, LogLevel logLevel) => m_filter(loggerName, logLevel);

	    private IDisposable BeginScope<TState>(TState state)
	    {
			// TODO: handle scopes
		    return null;
	    }

	    private readonly IStringLog m_stringLog;
	    private readonly Formatter m_formatter;
	    private readonly IsEnabledFilter m_filter;
	    private readonly ConcurrentDictionary<string, ILogger> m_loggers;
    }
}
