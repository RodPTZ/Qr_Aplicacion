using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core;
using MySqlConnector;
using Dapper;
using System.Data;

namespace SistemaDeBoleteria.AdoDapper;
public class TarifaAdo : ITarifaService
{
    private readonly IDbConnection db;
    public TarifaAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public TarifaAdo(IDbConnection dbConnection) => db = dbConnection;
    public TarifaAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

    public void InsertTarifa(Tarifa tarifa)
    {
        var sql = "INSERT INTO Tarifa (IdFuncion, Precio, Stock) VALUES (@IdFuncion, @Precio, @Stock);";
        var id = db.ExecuteScalar<int>(sql, tarifa);
        tarifa.IdTarifa = id;
    }
        
    public IEnumerable<Tarifa> GetTarifasByFuncionId(int idFuncion)
    {
        var sql = "SELECT * FROM Tarifa WHERE IdFuncion = @ID";
        return db.Query<Tarifa>(sql, new { ID = idFuncion });
    }
        
    public Tarifa? GetTarifaById(int id)
    {
        var sql = "SELECT * FROM Tarifa WHERE IdTarifa = @ID";
        return db.QueryFirstOrDefault<Tarifa>(sql, new { ID = id });
    }
        
    public void UpdateTarifa(Tarifa tarifa, int IdTarifa)
    {
        var sql = "UPDATE Tarifa SET Precio = @Precio, Stock = @Stock, Estado = @Estado WHERE IdTarifa = @ID";
        db.Execute(sql, new
        {
            tarifa.precio,
            tarifa.stock,
            tarifa.estado,
            ID = IdTarifa
        });
    }
        
}
