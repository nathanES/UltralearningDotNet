// using System.Data;
// using Npgsql;
// using TaskManagement.Tasks.Database;
//
// namespace TaskManagement.Tasks.Infrastructure.Database;
//
//Useless now that we use EF to initialize Database
// public class DbConnectionFactory : IDbConnectionFactory
// {
//     private readonly string _connectionString;
//
//     public DbConnectionFactory(string connectionString)
//     {
//         _connectionString = connectionString;
//     }
//
//     public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
//     {
//         var connection = new NpgsqlConnection(_connectionString);
//         await connection.OpenAsync(token);
//         return connection;
//     }
// }