using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class TestLogger : ILogger
    {
        private readonly List<string> _logMessages = new List<string>();

        public IReadOnlyList<string> LogMessages => _logMessages.AsReadOnly();

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                _logMessages.Add(formatter(state, exception));
            }
        }
    }
}
