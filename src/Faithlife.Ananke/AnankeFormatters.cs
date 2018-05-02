using System;
using System.Text;
using Faithlife.Ananke.Logging;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke
{
	/// <summary>
	/// Log formatters.
	/// </summary>
	public static class AnankeFormatters
    {
		/// <summary>
		/// A formatter that formats logs messages as structured text.
		/// </summary>
		/// <param name="loggerName">The name (category) of the logger. May not be <c>null</c>.</param>
		/// <param name="logLevel">The importance of the event.</param>
		/// <param name="eventId">The id of the event, or <c>0</c> if there is no id.</param>
		/// <param name="message">The message. May not be <c>null</c>, but may be the empty string.</param>
		/// <param name="exception">The exception, if any. May be <c>null</c>.</param>
	    public static string StructuredText(string loggerName, LogLevel logLevel, EventId eventId, string message, Exception exception)
	    {
		    if (message == "")
			    message = "Exception";

		    var sb = new StringBuilder();
		    sb.Append(StructuredTextLogLevel(logLevel));
		    sb.Append(Escaping.BackslashEscape(loggerName));
		    if (eventId.Id != 0)
			    sb.Append("(" + eventId.Id + ")");
			// TODO: scope goes here.
			sb.Append(": ");
		    sb.Append(Escaping.BackslashEscape(message));
		    if (exception != null)
		    {
			    sb.Append(": ");
			    sb.Append(Escaping.BackslashEscape(exception.ToString()));
		    }

		    return sb.ToString();
	    }

	    private static string StructuredTextLogLevel(LogLevel logLevel)
	    {
		    switch (logLevel)
		    {
		    case LogLevel.Trace:
			    return "T ";
		    case LogLevel.Debug:
			    return "D ";
		    case LogLevel.Information:
			    return "I ";
		    case LogLevel.Warning:
			    return "W ";
		    case LogLevel.Error:
			    return "E ";
		    case LogLevel.Critical:
			    return "C ";
		    }
			throw new InvalidOperationException($"Unknown LogLevel {logLevel}");
	    }
    }
}
