using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core;
using MySqlConnector;
using Dapper;
using System.Data;
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

    public IEnumerable<Entrada> GetEntradas()
        {
            var sql = "SELECT * FROM Entrada";
            return db.Query<Entrada>(sql);
        }
        public Entrada? GetEntradaById(int idEntrada)
        {
            var sql = "SELECT * FROM Entrada WHERE IdEntrada = @ID";
            return db.QueryFirstOrDefault<Entrada>(sql, new { ID = idEntrada });
        }
        public void AnularEntrada(int idEntrada)
        {
            var sql = "UPDATE Entrada SET Anulada = true WHERE IdEntrada = @ID";
            db.Execute(sql, new { ID = idEntrada });
        }
}
