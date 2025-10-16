using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core;
using MySqlConnector;
using Dapper;
using System.Data;

namespace SistemaDeBoleteria.AdoDapper;

public class LocalAdo : ILocalService
{
    private readonly IDbConnection db;
    public LocalAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public LocalAdo(IDbConnection dbConnection) => db = dbConnection;
    public LocalAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

    public IEnumerable<Local> GetLocales()
        {
            var sql = "SELECT * FROM Local";
            return db.Query<Local>(sql);
        }
        public Local? GetLocalById(int id)
        {
            var sql = "SELECT * FROM Local WHERE IdLocal = @ID";
            return db.QueryFirstOrDefault<Local>(sql, new { ID = id });
        }
        public void InsertLocal(Local local)
        {
            var sql = "INSERT INTO Local (Nombre, Ubicacion) VALUES (@Nombre, @Ubicacion);";
            var id = db.ExecuteScalar<int>(sql, local);
            local.IdLocal = id;
        }
        public void UpdateLocal(Local local)
        {
            var sql = "UPDATE Local SET Nombre = @Nombre, Ubicacion = @Ubicacion WHERE IdLocal = @ID";
            db.Execute(sql, local);
        }
        public bool DeleteLocal(int id)
        {
            var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdLocal = @ID";
            var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
            if (cantidadFunciones == 0)
            {
                var sql = "DELETE FROM Local WHERE IdLocal = @ID";
                db.Execute(sql, new { ID = id });
                return true;
            }
            return false;
        }
}
