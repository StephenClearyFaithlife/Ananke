using System;
using System.Collections.Generic;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <summary>
	/// Service that exits the entire process.
	/// </summary>
	public interface IExitProcessService
	{
		/// <summary>
		/// Gets or sets the exit code for the process.
		/// </summary>
		int ExitCode { get; set; }

		/// <summary>
		/// Exits the process with exit code <see cref="ExitCode"/>. This method may or may not return.
		/// </summary>
		void Exit();
	}

	/// <inheritdoc/>
    public sealed class ExitProcessService: IExitProcessService
	{
		/// <inheritdoc/>
		public int ExitCode
		{
			get => Environment.ExitCode;
			set => Environment.ExitCode = value;
		}

		/// <inheritdoc/>
		public void Exit() => Environment.Exit(ExitCode);
	}
}
