using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Logging
{
	/// <summary>
	/// A text writer that backslash-escapes all CRs and LFs, and only outputs a <c>NewLine</c> when an explicit <c>WriteLine</c> is requested.
	/// </summary>
    public sealed class EscapingTextWriter: TextWriter
	{
		/// <summary>
		/// Creates a new text writer that writes to the given inner text writer.
		/// </summary>
		/// <param name="writer">The wrapped text writer.</param>
		public EscapingTextWriter(TextWriter writer)
		{
			m_writer = writer;
		}

		/// <inheritdoc/>
		public override IFormatProvider FormatProvider => m_writer.FormatProvider;

		/// <inheritdoc/>
		public override string NewLine
		{
			get => m_writer.NewLine;
			set => m_writer.NewLine = value;
		}

		/// <inheritdoc/>
		public override Encoding Encoding => m_writer.Encoding;

		/// <inheritdoc/>
		public override void Flush() => m_writer.Flush();

		/// <inheritdoc/>
		public override Task FlushAsync() => m_writer.FlushAsync();

		/// <inheritdoc/>
		public override void Write(char value)
		{
			if (value == '\n')
				m_writer.Write("\\n");
			else if (value == '\r')
				m_writer.Write("\\r");
			else if (value == '\\')
				m_writer.Write("\\\\");
			else
				m_writer.Write(value);
		}

		/// <inheritdoc/>
		public override void WriteLine()
		{
			m_writer.WriteLine();
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

		private readonly TextWriter m_writer;
	}
}
