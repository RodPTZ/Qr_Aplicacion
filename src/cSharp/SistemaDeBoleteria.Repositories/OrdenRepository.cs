using SistemaDeBoleteria.Core.Models;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;
using System.Security.Cryptography.X509Certificates;
using SistemaDeBoleteria.Core.Enums;
namespace SistemaDeBoleteria.Repositories;

public class OrdenRepository :  DbRepositoryBase, IOrdenRepository
{
    public OrdenRepository(string connectionString) : base (connectionString){}

    const string InsSql = @"INSERT INTO Orden (IdTarifa, IdCliente, Emision, Cierre, MedioDePago)
                            VALUES (@IdTarifa, @IdCliente, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), @MedioDePago );

                            SELECT LAST_INSERT_ID();";
    public IEnumerable<Orden> SelectAll() => UseNewConnection(db => db.Query<Orden>("SELECT * FROM Orden"));
    public Orden? Select(int idOrden) => UseNewConnection(db => db.QueryFirstOrDefault<Orden>("SELECT * FROM Orden WHERE IdOrden = @ID", new { ID = idOrden }));
    
    public Orden Insert(Orden orden) => UseNewConnection(db =>
    {
        orden.IdOrden = db.ExecuteScalar<int>(InsSql, orden);
        return db.QueryFirstOrDefault<Orden>("SELECT * FROM Orden WHERE IdOrden = @ID", new { ID = orden.IdOrden })!;
    });
    #region Validación de negocio
    const string strUpdExpirado = @"UPDATE Orden
                                    SET Estado = 'Expirado'
                                    WHERE IdOrden = @ID";
    const string strUpdAbonado  = @"UPDATE Orden
                                    SET Estado = 'Abonado',
                                    Cierre = NOW()
                                    WHERE IdOrden = @ID;";
    const string strUpdCancelado = @" UPDATE Orden
                                      SET Estado = 'Cancelado',
                                      Cierre = NOW()
                                      WHERE IdOrden = @ID;";
    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Orden
                                             WHERE IdOrden = @ID)";
    const string strData = @"SELECT T.TipoEntrada, O.Estado, O.Cierre, F.Cancelado, T.Stock, T.Estado, E.Estado
                             FROM Orden O
                             JOIN Tarifa T USING (IdTarifa)
                             JOIN Funcion F USING (IdFuncion)
                             JOIN Evento E USING (IdEvento)
                             WHERE O.IdOrden = @ID";
    public bool UpdEstadoExpirado(int idOrden) => UseNewConnection(db => db.Execute(strUpdExpirado,new { ID = idOrden}) > 0);
    public bool UpdAbonado(int idOrden) => UseNewConnection(db => db.Execute(strUpdAbonado, new { ID = idOrden}) > 0);
    public bool UpdCancelado(int idOrden) => UseNewConnection(db => db.Execute(strUpdCancelado, new { ID = idOrden}) > 0);
    public bool Exists(int idOrden) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = idOrden}));
    public (ETipoEntrada, ETipoEstadoOrden, DateTime, bool, int, ETipoEstadoTarifa, ETipoEstadoEvento) Data(int unIdOrden) => UseNewConnection(db =>
    {
        return db.QueryFirst<(ETipoEntrada,ETipoEstadoOrden, DateTime, bool, int, ETipoEstadoTarifa, ETipoEstadoEvento)>(strData, new { ID = unIdOrden});
    });
    #endregion
}