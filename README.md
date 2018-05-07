# Ananke
A library to help .NET Core Console applications adhere to Docker conventions, named after the Greek goddess of inevitability.

[![Build status](https://ci.appveyor.com/api/projects/status/36q5usicy788pyes/branch/master?svg=true)](https://ci.appveyor.com/project/Faithlife/ananke/branch/master) 
[![NuGet](https://img.shields.io/nuget/v/Faithlife.Ananke.svg)](https://www.nuget.org/packages/Faithlife.Ananke)

# Sample Usage

Change your Console app's `Main` method to invoke `AnankeRunner.Main`, as such:

```C#
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

Ananke also passes an `ILoggerFactory` to your application logic called `AnankeContext.LoggerFactory`, which you can use to construct an `ILogger` and log to.

Finally, you should strongly consider setting `AnankeSettings.MaximumRuntime`. Giving this property a reasonable value will ensure your application will exit after a maximum amount of time. If your application is being orchestrated (e.g., in a Kubernetes Deployment), then you can set `MaximumRuntime` to create a "phoenix service" - one that periodically exits of its own free will and is then reborn by the orchestrator.

Taking these aspects into account, a more realistic example of Ananke usage is:

```C#
using Faithlife.Ananke;

class MyProgram
{
  private static readonly AnankeSettings Settings = AnankeSettings.Create(maximumRuntime: TimeSpan.FromHours(2));

  static void Main(string[] args) => AnankeRunner.Main(Settings, async context =>
  {
    // Normally loggers are created by dependency injection; this sample just creates it directly.
    var logger = context.LoggerFactory.CreateLogger<MyProgram>();

    while (true)
    {
      // Wait for the next work item to be available, and retrieve it.
      // If we are requested to exit, then cancel the wait.
      var workItem = await GetNextWorkItemAsync(context.ExitRequested);

      // Process the work item. Ignore requests to exit.
      logger.LogInformation("Processing {workItemId}", workItem.Id);
      ProcessWorkItem(workItem);
    }
  });
}
```

The actual shutdown time is randomly "fuzzed" a bit by default (see `AnankeSettings.RandomMaximumRuntimeRelativeDelta`). The default value for this is `0.1` (i.e., +/- 10%), so the *actual* maximum runtime of the code above will vary by +/- 12 minutes (10% of 2 hours), and be a random value between 1:48 and 2:12. This is just to avoid all applications from exiting at the exact same time, even if they were all started at the same time.

# Doker Conventions

## Exit Codes

An Ananke process will return one of the following exit codes:

* `0` - If the application logic returns without exception.
* `64` - If the application logic directly threw an unhandled exception. Unhandled exceptions are logged before the process exits.
* `65` - If the application logic indirectly threw an unhandled exception (e.g., from a thread pool thread). Unhandled exceptions are logged before the process exits.
* `66` - If the application logic was requested to shutdown, but did not do so within the exit timeout (see `AnankeSettings.ExitTimeout`).
* (other) - If the application logic returns an `int`, then that value is used as the process exit code.

Exit codes are returned by Ananke even if you use `static void Main` as your entrypoint.

## Signals

Ananke responds to `Ctrl-C` (if your container is run interactively) as well as `docker stop` on all platforms and container types. Note that for `docker stop` to work with Windows containers, they must have a base image *and* host running Windows Version `1709` or higher.

### Signal Specifics

Ananke listens to various signals based on OS:
* Windows: `CTRL_C_EVENT`, `CTRL_CLOSE_EVENT`, and `CTRL_SHUTDOWN_EVENT`
* Other: `SIGINT` (`Ctrl-C`) and `SIGTERM`

These are all treated the same: as a graceful stop request. When one of these signals is received, the `AnankeContext.ExitRequested` cancellation token is cancelled. When this token is cancelled, your code should stop taking on new work. It should complete the work it already has and then exit.

When a signal comes in, Ananke will start a kill timer (see `AnankeSettings.ExitTimeout`). If the application code has not returned within that timeout, Ananke will exit the process with exit code `66`.

## Logs

Docker expects logs to be written to stdout (or stderr), with *one line per log message*.

Ananke has a core logging factory, exposed at `AnankeContext.LoggingFactory`. All of Ananke's logs go through this factory (using the `"Ananke"` category/logger name), and this same factory can be used to create application logs.

By default, all log messages sent to `AnankeContext.LoggingFactory` are formatted on a single line using backslash-escaping. These lines are then written to stdout.

### Redirecting Ananke Logs

Docker applications that deliberately do their own logging directly will want to redirect Ananke's logging. This is done by setting `AnankeSettings.LoggingFactory` before calling into Ananke. Ananke will then use the provided `ILoggerFactory` instead of its own factory and provider.

```C#
static void Main(string[] args)
{
	var myLoggerFactory = new LoggerFactory();
	myLoggerFactory.AddMyOwnProvider(); // log4net, seq, gelf, whatever...

	var anankeSettings = AnankeSettings.Create(maximumRuntime: TimeSpan.FromHours(2), loggerFactory: myLoggerFactory);
 	AnankeRunner.Main(anankeSettings, context =>
	{
		var loggerFactory = context.LoggerFactory; // Same instance as `myLoggerFactory` that we passed into the settings.
	});
}
```

This way you can send Ananke's own logs to your customized logging provider instead of as stdout to Docker.

### Intercepting Console Stdout and Stderr (Advanced)

Ananke does *not* intercept any application-level logs by default. Any direct console output from application logic is passed straight through. It is possible to intercept console output as such:

```C#
static void Main(string[] args) => AnankeRunner.Main(AnankeSettings.Create(), context =>
{
	Console.SetOut(context.LoggingConsoleStdout);
	Console.SetError(context.LoggingConsoleStdout);

	Console.WriteLine("Hello\nWorld!"); // Formatted and written to stdout as one line, not two
});
```

Please note that intercepted console outputs *require* the use of `WriteLine`. Code such as `Console.Write("Hello World!\n")` will write `Hello World!\\n`, not `Hello World!\n`, and will be interpreted as a log message that has not yet completed.

Redirected console output using `AnankeContext.LoggingConsoleStdout` captures all writes, and when `WriteLine` is invoked, it sends the log string to `AnankeContext.LoggingFactory`. If you specify a custom `AnankeSettings.LoggingFactory`, then you can use this technique to redirect all `Console.WriteLine` calls to your own logging provider.
