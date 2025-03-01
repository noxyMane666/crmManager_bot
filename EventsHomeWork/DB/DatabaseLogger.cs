using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.DB
{
    public class DatabaseLogger(string connectionString) : ILogger
    {
        private readonly string _connectionString = connectionString;

        IDisposable? ILogger.BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);
            Task.Run(() => SaveLogToDatabase(logLevel, message, exception?.ToString() ?? "Нет исключения"));
        }

        private async Task SaveLogToDatabase(LogLevel logLevel, string message, string exception)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "INSERT INTO Logs (LogLevel, Message, Exception, CreatedAt) VALUES (@LogLevel, @Message, @Exception, @CreatedAt)";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@LogLevel", logLevel.ToString());
            command.Parameters.AddWithValue("@Message", message);
            command.Parameters.AddWithValue("@Exception", exception?.ToString() ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await command.ExecuteNonQueryAsync();
        }
    }
}
