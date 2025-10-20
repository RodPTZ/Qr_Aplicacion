using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.AdoDapper;

public class EntradaAdo : IEntradaService
{
    private readonly IDbConnection db;
    public EntradaAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public EntradaAdo(IDbConnection dbConnection) => db = dbConnection;
    public EntradaAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

    public IEnumerable<MostrarEntradaDTO> GetEntradas()
    {
        var sql = "SELECT * FROM Entrada";
        return db.Query<MostrarEntradaDTO>(sql);
    }
    public MostrarEntradaDTO? GetEntradaById(int idEntrada)
    {
        var sql = "SELECT * FROM Entrada WHERE IdEntrada = @ID";
        return db.QueryFirstOrDefault<MostrarEntradaDTO>(sql, new { ID = idEntrada });
    }
    public void AnularEntrada(int idEntrada)
    {
        // var sql = "UPDATE Entrada SET Anulada = true WHERE IdEntrada = @ID";
        // db.Execute(sql, new { ID = idEntrada });
        var parameters = new DynamicParameters();
        parameters.Add("@unIdEntrada", idEntrada);
        db.Execute("CancelarEntrada", parameters);

    }
}
