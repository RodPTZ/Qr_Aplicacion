using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
namespace SistemaDeBoleteria.Core.Inheritance
{
    // public abstract class DbRepositoryBase
    // {
    //     private readonly string UseConnection;
    //     public DbRepositoryBase(string connectionString){ 
    //         if (string.IsNullOrEmpty(connectionString))
    //         {
    //             throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(connectionString));
    //         }
    //         UseConnection = connectionString;}
    //     protected T UseNewConnection<T>(Func<IDbConnection, T> newConnection)
    //     {
    //         using var connection = new MySqlConnection(UseConnection);
    //         connection.Open();
    //         return newConnection(connection);
    //     }
    // }
    public abstract class DbRepositoryBase
    {
        // protected readonly IDbConnection db;
        // public DbRepositoryBase(string connectionString) => db = new MySqlConnection(connectionString);
        // public DbRepositoryBase(IDbConnection dbConnection) => db = dbConnection;
        protected T UseNewConnection<T>(Func<IDbConnection, T> func)
        {
            using var conn = new MySqlConnection($"Server=localhost;Database=5to_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
            conn.Open();
            return func(conn);
        }
    }
}
            // db = new MySqlConnection($"Server=localhost;Database=5to_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
