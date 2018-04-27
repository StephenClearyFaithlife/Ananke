# Ananke
A library to help .NET Core Console applications adhere to Docker conventions, named after the Greek goddess of inevitability.

# Sample Usage

Change your Console app's `Main` method to invoke `AnankeRunner.Main`, as such:

```
using Faithlife.Ananke;

class Program
{
	static void Main(string[] args) => AnankeRunner.Main(AnankeSettings.Create(), context =>
	{
		Console.WriteLine("Hello World!");
	});
}
```

There are three Ananke types used in this code:

1. The code creates an `AnankeSettings` object with the default settings. These settings control how Ananke behaves.
1. The code invokes `AnankeRunner.Main`, passing it the settings object and delegate representing the application logic.
1. The application logic now receives an `AnankeContext` object with its execution context.

# Realistic Usage

Ananke passes a `CancellationToken` to your application logic called `AnankeContext.ExitRequested`. This token is cancelled whenever your application is requested to shut down. When `ExitRequested` is cancelled, your application logic should stop taking on new work, finish processing the current work it already has, and then return. If it does not do this, then its processing will be aborted when the application exits.

Also, you should strongly consider setting `AnankeSettings.MaximumRuntime`. Giving this property a reasonable value will ensure your application will exit after a maximum amount of time. If your application is being orchestrated (e.g., in a Kubernetes Deployment), then you can set `MaximumRuntime` to create a "phoenix service" - one that periodically exits of its own free will and is then reborn by the orchestrator.

Taking these aspects into account, a more realistic example of Ananke usage is:

```
using Faithlife.Ananke;

class Program
{
	private static readonly AnankeSettings Settings = AnankeSettings.Create(maximumRuntime: TimeSpan.FromHours(2));

	static void Main(string[] args) => AnankeRunner.Main(Settings, async context =>
	{
		while (!context.ExitRequested.IsCancellationRequested)
		{
			// Wait for the next work item to be available, and retrieve it.
			// If we are requested to exit, then cancel the wait.
			var workItem = await GetNextWorkItemAsync(context.ExitRequested);

			// Process the work item. Ignore requests to exit.
			ProcessWorkItem(workItem);
		}
	});
}
```

# Doker Conventions

## Exit Codes

An Ananke process will return one of the following exit codes:

* `0` - If the application logic returns without exception.
* `64` - If the application logic threw an unhandled exception. Unhandled exceptions are logged before the process exits.
* `65` - If the application logic was requested to shutdown, but did not do so within the exit timeout (see `AnankeSettings.ExitTimeout`).
* (other) - If the application logic returns an `int`, then that value is used as the process exit code.

Exit codes are returned by Ananke even if you use `static void Main` as your entrypoint.

## Signals

Ananke listens to `SIGINT` (`Ctrl-C`) and `SIGTERM` (`docker stop`). When one of these signals is received, the `AnankeContext.ExitRequested` cancellation token is cancelled. When this token is cancelled, your code should stop taking on new work. It should complete the work it already has and then exit.

Both `SIGINT` and `SIGTERM` are treated as graceful stop requests. However, for both signals, Ananke will start a kill timer (see `AnankeSettings.ExitTimeout`). If the application code has not returned within that timeout, Ananke will exit the process with exit code `65`.

## Logs

Docker expects logs to be written to stdout (or stderr), with *one line per log message*.

Ananke formats logs messages on a single line using backslash-escaping. It then writes the log messages to stdout through a logging service (see `AnankeSettings.ConsoleLogService`).

### Intercepting Console Stdout and Stderr

Ananke does *not* intercept any application-level logs by default. Any direct console output from application logic is passed straight through. It is possible to intercept console output as such:

```
static void Main(string[] args) => Runner.Main(Settings.Create(), context =>
{
	Console.SetOut(context.EscapedConsoleStdout);
	Console.SetError(context.EscapedConsoleStderr);

	Console.WriteLine("Hello\nWorld!"); // Written to stdout as one line, not two
});
```

Please note that intercepted console outputs *require* the use of `WriteLine`. Code such as this will write `Hello World!\\n` to the console, not `Hello World!\n`, and will be interpreted by Docker as a log message that has not yet completed:

```
Console.Write("Hello World!\n");
```
