﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Faithlife.Ananke.Logging
{
	/// <summary>
	/// Provides logger extensions used by Ananke.
	/// </summary>
    internal static class InternalLoggerExtensions
    {
		// Category: Ananke
		// See https://github.com/aspnet/Logging/issues/762#issuecomment-366876458 / http://www.webcitation.org/6z7HJFsDE

		public static void MaximumRuntime(this ILogger logger, TimeSpan shutdownAfter) => s_maximumRuntime(logger, shutdownAfter, null);
	    public static void MaximumRuntimeReached(this ILogger logger, TimeSpan shutdownAfter) => s_maximumRuntimeReached(logger, shutdownAfter, null);
	    public static void ShutdownSignalReceived(this ILogger logger, string shutdownSignal) => s_shutdownSignalReceived(logger, shutdownSignal, null);
	    public static void UnhandledAppDomainException(this ILogger logger, Exception exception) => s_unhandledAppDomainException(logger, exception);
	    public static void UnhandledApplicationException(this ILogger logger, Exception exception) => s_unhandledApplicationException(logger, exception);
	    public static void IgnoringOperationCanceledException(this ILogger logger) => s_ignoringOperationCanceledException(logger, null);

		private static readonly Action<ILogger, TimeSpan, Exception> s_maximumRuntime =
		    LoggerMessage.Define<TimeSpan>(LogLevel.Information, new EventId(1, nameof(MaximumRuntime)), "Maximum runtime set to {shutdownAfter}.");
	    private static readonly Action<ILogger, TimeSpan, Exception> s_maximumRuntimeReached =
		    LoggerMessage.Define<TimeSpan>(LogLevel.Information, new EventId(2, nameof(MaximumRuntimeReached)), "Shutting down (Maximum runtime of {shutdownAfter} reached).");
	    private static readonly Action<ILogger, string, Exception> s_shutdownSignalReceived =
		    LoggerMessage.Define<string>(LogLevel.Information, new EventId(3, nameof(ShutdownSignalReceived)), "Shutting down (Shutdown signal {shutdownSignal} received).");
	    private static readonly Action<ILogger, Exception> s_unhandledAppDomainException =
		    LoggerMessage.Define(LogLevel.Error, new EventId(4, nameof(UnhandledAppDomainException)), "Unhandled AppDomain exception.");
	    private static readonly Action<ILogger, Exception> s_unhandledApplicationException =
		    LoggerMessage.Define(LogLevel.Error, new EventId(5, nameof(UnhandledApplicationException)), "Unhandled application exception.");
	    private static readonly Action<ILogger, Exception> s_ignoringOperationCanceledException =
		    LoggerMessage.Define(LogLevel.Debug, new EventId(6, nameof(UnhandledApplicationException)), "Ignoring OperationCanceledException since we are shutting down.");
    }
}