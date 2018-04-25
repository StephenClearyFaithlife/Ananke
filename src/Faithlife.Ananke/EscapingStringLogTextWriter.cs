using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke
{
	/// <summary>
	/// A text writer that backslash-escapes all CRs and LFs, and only outputs a <c>NewLine</c> when an explicit <c>WriteLine</c> is requested.
	/// </summary>
    public sealed class EscapingStringLogTextWriter: TextWriter
	{
		/// <summary>
		/// Creates a new text writer that writes to the given string log.
		/// </summary>
		/// <param name="stringLog">The output string log.</param>
		public EscapingStringLogTextWriter(IStringLogService stringLog)
		{
			m_stringLog = stringLog;
			m_buffer = new StringBuilder();
		}

		/// <inheritdoc/>
		public override Encoding Encoding => Encoding.UTF8;

		/// <inheritdoc/>
		public override void Write(char value)
		{
			if (value == '\n')
				m_buffer.Append("\\n");
			else if (value == '\r')
				m_buffer.Append("\\r");
			else if (value == '\\')
				m_buffer.Append("\\\\");
			else
				m_buffer.Append(value);
		}

		/// <inheritdoc/>
		public override void WriteLine()
		{
			m_stringLog.WriteLine(m_buffer.ToString());
			m_buffer.Clear();
		}

		/// <inheritdoc/>
		public override void WriteLine(bool value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char[] buffer)
		{
			Write(buffer);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			Write(buffer, index, count);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(decimal value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(double value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(int value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(long value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(object value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(float value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0)
		{
			Write(format, arg0);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0, object arg1)
		{
			Write(format, arg0, arg1);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			Write(format, arg0, arg1, arg2);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(string format, params object[] arg)
		{
			Write(format, arg);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(uint value)
		{
			Write(value);
			WriteLine();
		}

		/// <inheritdoc/>
		public override void WriteLine(ulong value)
		{
			Write(value);
			WriteLine();
		}

		private readonly IStringLogService m_stringLog;
		private readonly StringBuilder m_buffer;
	}
}
