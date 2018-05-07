using System;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Faithlife.Ananke;

namespace TimerSample
{
	class Program
	{
		private static readonly AnankeSettings Settings = AnankeSettings.Create(maximumRuntime: TimeSpan.FromSeconds(30));

		static void Main() => AnankeRunner.Main(Settings, async context =>
		{
			// Normally this is injected by your DI container; this example just creates it directly.
			var logger = context.LoggerFactory.CreateLogger<Program>();

			while (true)
			{
				await Task.Delay(TimeSpan.FromSeconds(1), context.ExitRequested);
				logger.LogInformation("Hello World!");
			}
		});
	}
}
