using System;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Ananke;

namespace TimerSample
{
    class Program
    {
	    private static readonly AnankeSettings Settings = AnankeSettings.Create(maximumRuntime: TimeSpan.FromSeconds(30));

		static void Main() => AnankeRunner.Main(Settings, async context =>
		{
			Console.SetOut(context.LoggingConsoleStdout);
			Console.SetError(context.LoggingConsoleStdout);

			while (true)
			{
				await Task.Delay(TimeSpan.FromSeconds(1), context.ExitRequested);
				Console.WriteLine("Hello World!");
			}
		});
	}
}
