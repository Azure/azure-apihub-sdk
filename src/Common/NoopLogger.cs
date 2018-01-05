using System;
using System.Diagnostics;

namespace Microsoft.Azure.ApiHub
{
    internal class NoopLogger : ILogger
    {
        public static ILogger Instance = new NoopLogger();

        private NoopLogger()
        { }

        public TraceLevel Level { get; set; }

        public void Error(string message, Exception ex = null, string source = null)
        {
            // no-op
        }

        public void Info(string message, string source = null)
        {
            // no-op
        }

        public void Verbose(string message, string source = null)
        {
            // no-op
        }

        public void Warning(string message, string source = null)
        {
            // no-op
        }
    }
}
