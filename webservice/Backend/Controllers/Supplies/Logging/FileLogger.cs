using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();

        public FileLogger(string path)
        {
            filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //return logLevel == LogLevel.Trace;
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    string message = String.Format("{0} {1}: {2}", DateTime.Now.ToString(), logLevel.ToString().ToUpper(), state);
                    File.AppendAllText(filePath, message/*formatter(state, exception)*/ + Environment.NewLine);
                }
            }
        }
    }
}