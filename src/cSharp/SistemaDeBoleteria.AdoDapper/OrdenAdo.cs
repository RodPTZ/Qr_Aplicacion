using SistemaDeBoleteria.Core.Models;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Services;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;

namespace SistemaDeBoleteria.AdoDapper;

public class OrdenAdo : IOrdenService
{
    private readonly IDbConnection db;
    public OrdenAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public OrdenAdo(IDbConnection dbConnection) => db = dbConnection;
    public OrdenAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

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
