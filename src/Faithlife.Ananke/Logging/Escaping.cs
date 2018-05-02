using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Faithlife.Ananke.Logging
{
	/// <summary>
	/// Utility class to backslash-escape character sets.
	/// </summary>
    public static class Escaping
	{
		/// <summary>
		/// Backslash-escapes the newline characters in a source string.
		/// </summary>
		/// <param name="source">The source string.</param>
	    public static string BackslashEscape(string source)
	    {
		    if (source.IndexOfAny(s_backslashEscapeChars) == -1)
			    return source;

			var sb = new StringBuilder(source.Length);
		    foreach (var ch in source)
		    {
			    if (ch == '\\')
				    sb.Append("\\\\");
				else if (ch == '\n')
				    sb.Append("\\n");
				else if (ch == '\r')
				    sb.Append("\\r");
			    else
				    sb.Append(ch);
		    }

		    return sb.ToString();
	    }

		private static readonly char[] s_backslashEscapeChars = {'\\', '\n', '\r'};
	}
}
