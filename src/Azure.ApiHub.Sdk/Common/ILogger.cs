// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Microsoft.Azure.ApiHub
{
    /// <summary>
    /// Logging interface for ApiHub SDK
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets or sets the <see cref="TraceLevel"/> filter used to filter trace events.
        /// Only trace events with a <see cref="TraceLevel"/> less than or equal to
        /// this level will be traced.
        /// <remarks>
        /// Level filtering will be done externally by the <see cref="JobHost"/>, so
        /// it shouldn't be done by this class.
        /// </remarks>
        /// </summary>
        TraceLevel Level { get; set; }

        /// <summary>
        /// Writes a <see cref="TraceLevel.Verbose"/> level trace event.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="source">The source of the message.</param>
        void Verbose(string message, string source = null);

        /// <summary>
        /// Writes a <see cref="TraceLevel.Info"/> level trace event.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="source">The source of the message.</param>
        void Info(string message, string source = null);

        /// <summary>
        /// Writes a <see cref="TraceLevel.Warning"/> level trace event.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="source">The source of the message.</param>
        void Warning(string message, string source = null);

        /// <summary>
        /// Writes a <see cref="TraceLevel.Error"/> level trace event.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="ex">The optional <see cref="Exception"/> for the error.</param>
        /// <param name="source">The source of the message.</param>
        void Error(string message, Exception ex = null, string source = null);
    }
}
