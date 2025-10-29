using SistemaDeBoleteria.Core.Models;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;
using SistemaDeBoleteria.Core.Inheritance;
namespace SistemaDeBoleteria.Repositories;

public class OrdenRepository :  DbRepositoryBase, IOrdenRepository
{
    public IEnumerable<MostrarOrdenDTO> GetOrdenes()
    {
        var sql = "SELECT * FROM Orden";
        return db.Query<MostrarOrdenDTO>(sql);
    }
    public MostrarOrdenDTO? GetOrdenById(int id)
    {
        var sql = "SELECT * FROM Orden WHERE IdOrden = @ID";
        return db.QueryFirstOrDefault<MostrarOrdenDTO>(sql, new { ID = id });
    }
    public MostrarOrdenDTO InsertOrden(CrearOrdenDTO orden)
    {
        var sql = "INSERT INTO Orden (IdCliente, IdSesion, TipoEntrada, MedioDePago, Emision, Cierre, Abonado, Cancelado) VALUES (@IdCliente, @IdSesion, @TipoEntrada, @MedioDePago, NOW(),  DATE_ADD(NOW(), INTERVAL 15 MINUTE));";
        var id = db.ExecuteScalar<int>(sql, new
        {
            orden.IdCliente,
            orden.IdSesion,
            TipoEntrada = orden.tipoEntrada.ToString(),
            orden.MedioDePago
        });
        var ordenCreada = orden.Adapt<MostrarOrdenDTO>();
        ordenCreada.IdOrden = id;
        return ordenCreada;
    }
    public bool PagarOrden(int id)
    {
        var sql = "UPDATE Orden SET Abonado = true WHERE IdOrden = @ID";
        db.Execute(sql, new { ID = id });
        var insEntrada = "INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez) VALUES (@ID,(SELECT TipoEntrada FROM Orden WHERE IdOrden = @ID),NOW(),(SELECT Cierre FROM Sesion WHERE IdSesion = (SELECT IdSesion FROM Orden WHERE IdOrden = @ID)));";
        db.Execute(insEntrada, new { ID = id });
        var insQR = "INSERT INTO CodigoQR (IdEntrada, Codigo) VALUES (@ID,(SELECT CONCAT_WS('-', Nombre, IdOrden, Emision, TipoEntrada, DNI) FROM Entrada WHERE IdEntrada = @ID));";
        db.Execute(insQR, new { ID = id });
        return true;
    }
    public bool CancelarOrden(int id)
    {
        var sql = "UPDATE Orden SET Cancelado = true WHERE IdOrden = @ID";
        db.Execute(sql, new { ID = id });
        return true;
    }
    
}
