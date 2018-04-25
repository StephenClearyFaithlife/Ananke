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

Ananke formats logs messages on a single line using backslash-escaping. It then writes the log messages to stdout through a logging service (see `IStringLogService`).

Ananke does *not* intercept any application-level logs by default. Any application logic that writes directly to the console is passed straight through.

## Exit Codes

An Ananke process will return one of the following exit codes:

* `0` - If the application logic returns without exception.
* `64` - If the application logic threw an unhandled exception.

However, if the application logic returns an `int`, then that is used as the process exit code.

Exit codes are returned by Ananke even if you use `static void Main` as your entrypoint.

