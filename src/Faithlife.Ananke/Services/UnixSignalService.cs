using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Faithlife.Ananke.Services
{
	/// <inheritdoc />
	public sealed class UnixSignalService : ISignalService
	{
		/// <inheritdoc />
		public void AddHandler(Action<string> handler)
		{
			Console.CancelKeyPress += (_, args) =>
			{
				if (args.SpecialKey == ConsoleSpecialKey.ControlC)
				{
					args.Cancel = true;
					handler("SIGINT");
				}
			};

			// See https://github.com/dotnet/coreclr/issues/7394
			var assemblyLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
			assemblyLoadContext.Unloading += _ => handler("SIGTERM");
		}
	}
}
