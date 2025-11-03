using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;
namespace SistemaDeBoleteria.Core.Inheritance
{
    public abstract class DbRepositoryBase
    {
        protected readonly IDbConnection db;
        public DbRepositoryBase(string connectionString) => db = new MySqlConnection(connectionString);
        public DbRepositoryBase(IDbConnection dbConnection) => db = dbConnection;
        public DbRepositoryBase()
        {
            db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
        }
        protected T Exec<T>(Func<IDbConnection, T> func)
        {
           
            using var conn = new MySqlConnection();
            conn.Open();
            return func(conn);
        }
    }
}