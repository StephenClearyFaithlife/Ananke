using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <summary>
	/// Service that writes strings to a log.
	/// </summary>
	public interface IStringLogService
	{
		/// <summary>
		/// Writes a single message to the log.
		/// </summary>
		/// <param name="message">The message to write.</param>
		void WriteLine(string message);
	}

	/// <inheritdoc/>
	/// This type writes all string messages to a text writer, without any escaping.
    public sealed class TextWriterStringLogService : IStringLogService
	{
		/// <summary>
		/// Creates a string logger that logs to a text writer.
		/// </summary>
		/// <param name="textWriter">The text writer.</param>
		public TextWriterStringLogService(TextWriter textWriter)
		{
			m_textWriter = textWriter;
		}

		/// <inheritdoc/>
		public void WriteLine(string message) => m_textWriter.WriteLine(message);

		private readonly TextWriter m_textWriter;
	}
}
