using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;

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

    public IEnumerable<MostrarLocalDTO> GetLocales()
    {
        var sql = "SELECT * FROM Local";
        return db.Query<MostrarLocalDTO>(sql);
    }
    public MostrarLocalDTO? GetLocalById(int id)
    {
        var sql = "SELECT * FROM Local WHERE IdLocal = @ID";
        return db.QueryFirstOrDefault<MostrarLocalDTO>(sql, new { ID = id });
    }
    public MostrarLocalDTO InsertLocal(CrearActualizarLocalDTO local)
    {
        var sql = "INSERT INTO Local (Nombre, Ubicacion) VALUES (@Nombre, @Ubicacion);";
        var id = db.ExecuteScalar<int>(sql, local);
        var localCreado = local.Adapt<MostrarLocalDTO>();
        localCreado.IdLocal = id;
        return localCreado;
    }
    public MostrarLocalDTO UpdateLocal(CrearActualizarLocalDTO local, int IdLocal)
    {
        var sql = "UPDATE Local SET Nombre = @Nombre, Ubicacion = @Ubicacion WHERE IdLocal = @ID";
        db.Execute(sql, local);
        var localActualizado = local.Adapt<MostrarLocalDTO>();
        localActualizado.IdLocal = IdLocal;
        return localActualizado;
    }
    public bool DeleteLocal(int IdLocal)
    {
        var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdLocal = @ID";
        var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = IdLocal });
        if (cantidadFunciones == 0)
        {
            var sql = "DELETE FROM Local WHERE IdLocal = @ID";
            db.Execute(sql, new { ID = IdLocal });
            return true;
        }
        return false;
    }
}
