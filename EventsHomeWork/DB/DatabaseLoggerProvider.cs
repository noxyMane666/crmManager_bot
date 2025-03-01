using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EventsHomeWork.DB
{

    public class DatabaseLoggerProvider(string connectionString) : ILoggerProvider
    {
        private readonly string _connectionString = connectionString;
        private readonly ConcurrentDictionary<string, DatabaseLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new DatabaseLogger(_connectionString));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

}
