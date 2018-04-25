# Ananke
A library to help .NET Core Console applications adhere to Docker conventions, named after the Greek goddess of inevitability.

# Sample Usage

Change your Console app's `Main` method to invoke `Runner.Main`, as such:

```
using Faithlife.Ananke;

class Program
{
	static void Main(string[] args) => Runner.Main(Settings.Create(), context =>
	{
		Console.WriteLine("Hello World!");
	});
}
```

There are two main aspects to this code:

1. It creates a `Settings` object with the default settings.
1. The application logic now receives a `Context` object with its execution context.

TODO: Describe the important parts of `Settings` and `Context`.

# Doker Conventions

## Logs

Docker expects logs to be written to stdout (or stderr), with *one line per log message*.

Ananke formats logs messages on a single line using backslash-escaping. It then writes the log messages to stdout through a logging service (see `Settings.ConsoleLogService`).

### Intercepting Console Stdout and Stderr

Ananke does *not* intercept any application-level logs by default. Any direct console output from application logic is passed straight through. It is possible to redirect console output by calling `Context.HookConsoleOutputs` as such:

```
static void Main(string[] args) => Runner.Main(Settings.Create(), context =>
{
	context.InterceptConsoleOutputs();
	Console.WriteLine("Hello\nWorld!"); // Reaches the console as one line, not two
});
```

Please note that hooked console outputs *require* the use of `WriteLine`. Code such as this will not work:

```
Console.Write("Hello World!\n"); // Considered an incomplete message; not written to Console.
```

## Exit Codes

An Ananke process will return one of the following exit codes:

* `0` - If the application logic returns without exception.
* `64` - If the application logic threw an unhandled exception.

However, if the application logic returns an `int`, then that is used as the process exit code.

Exit codes are returned by Ananke even if you use `static void Main` as your entrypoint.

## Signals

Ananke listens to `SIGINT` (Ctrl-C) and `SIGTERM` (`docker stop`). When one of these signals is received, the `Context.ExitRequested` cancellation token is cancelled. When this token is cancelled, your code should stop taking on new work. It should complete the work it already has and then exit.
