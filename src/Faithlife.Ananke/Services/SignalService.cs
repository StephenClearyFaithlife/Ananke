using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <summary>
	/// Provides a way to handle process signals.
	/// </summary>
	public interface ISignalService
	{
		/// <summary>
		/// Adds a handler to this process signal. There is not currently a way to remove a handler.
		/// </summary>
		/// <param name="handler">The handler to add.</param>
		void AddHandler(Action handler);
	}

	/// <inheritdoc />
	public sealed class SigintSignalService : ISignalService
	{
		/// <inheritdoc />
		public void AddHandler(Action handler)
		{
			Console.CancelKeyPress += (_, args) =>
			{
				if (args.SpecialKey == ConsoleSpecialKey.ControlC)
				{
					args.Cancel = true;
					handler();
				}
			};
		}
	}

	/// <inheritdoc />
	public sealed class SigtermSignalService : ISignalService
	{
		/// <inheritdoc />
		public void AddHandler(Action handler)
		{
			// See https://github.com/dotnet/coreclr/issues/7394
			var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
			assemblyLoadContext.Unloading += _ => handler();
		}
	}
}
