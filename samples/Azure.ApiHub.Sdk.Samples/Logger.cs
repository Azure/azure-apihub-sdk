using Microsoft.Azure.ApiHub;
using System;
using System.Diagnostics;

namespace Azure.ApiHub.Sdk.Samples
{
    class ConsoleLogger : ILogger
    {
        public TraceLevel Level
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Error(string message, Exception ex = null, string source = null)
        {
            Console.WriteLine("Error: " + message);

            if (ex != null)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public void Info(string message, string source = null)
        {
            Console.WriteLine("Info: " + message);
        }

        public void Verbose(string message, string source = null)
        {
            Console.WriteLine("Verbose: " + message);
        }

        public void Warning(string message, string source = null)
        {
            Console.WriteLine("Warning: " + message);
        }
    }
}
