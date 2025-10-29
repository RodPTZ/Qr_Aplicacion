using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;
using SistemaDeBoleteria.Core.Inheritance;
namespace SistemaDeBoleteria.Repositories;
public class TarifaRepository :  DbRepositoryBase, ITarifaService
{
    public MostrarTarifaDTO InsertTarifa(CrearTarifaDTO tarifa)
    {
        var sql = "INSERT INTO Tarifa (IdFuncion, Precio, Stock) VALUES (@IdFuncion, @Precio, @Stock);";
        var id = db.ExecuteScalar<int>(sql, tarifa);
        var mostrarTarifa = tarifa.Adapt<MostrarTarifaDTO>();
        mostrarTarifa.IdTarifa = id;
        return mostrarTarifa;
    }
        
    public IEnumerable<MostrarTarifaDTO> GetTarifasByFuncionId(int idFuncion)
    {
        var sql = "SELECT * FROM Tarifa WHERE IdFuncion = @ID";
        return db.Query<MostrarTarifaDTO>(sql, new { ID = idFuncion });
    }
        
    public MostrarTarifaDTO? GetTarifaById(int id)
    {
        var sql = "SELECT * FROM Tarifa WHERE IdTarifa = @ID";
        return db.QueryFirstOrDefault<MostrarTarifaDTO>(sql, new { ID = id });
    }
        
    public MostrarTarifaDTO UpdateTarifa(ActualizarTarifaDTO tarifa, int IdTarifa)
    {
        var sql = "UPDATE Tarifa SET Precio = @Precio, Stock = @Stock, Estado = @Estado WHERE IdTarifa = @ID";
        db.Execute(sql, new
        {
            tarifa.precio,
            tarifa.stock,
            tarifa.estado,
            ID = IdTarifa
        });
        return GetTarifaById(IdTarifa)!;
    }
        
}
