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

    const string InsSql = @"INSERT INTO Orden (IdTarifa, IdCliente, IdFuncion, Emision, Cierre, MedioDePago)
                            VALUES (@IdTarifa, @IdCliente, @IdFuncion, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), @MedioDePago );

                            SELECT LAST_INSERT_ID();";
    public IEnumerable<Orden> SelectAll() => UseNewConnection(db => db.Query<Orden>("SELECT * FROM Orden"));
    public Orden? Select(int idOrden) => UseNewConnection(db => db.QueryFirstOrDefault<Orden>("SELECT * FROM Orden WHERE IdOrden = @ID", new { ID = idOrden }));
    
    public Orden Insert(Orden orden) => UseNewConnection(db =>
    {
        
        // var parameters = new DynamicParameters();
        // parameters.Add("@unIdOrden", direction: ParameterDirection.Output);
        // parameters.Add("@unIdTarifa", orden.IdTarifa);
        // parameters.Add("@unIdCliente", orden.IdCliente);
        // parameters.Add("@unIdFuncion", orden.IdFuncion);
        // parameters.Add("@unMedioDePago", orden.MedioDePago);

        // db.Execute("AltaOrden", parameters);
        // orden.IdOrden = parameters.Get<int>("@unIdOrden");
        orden.IdOrden = db.ExecuteScalar<int>(InsSql, orden);
        return db.QueryFirstOrDefault<Orden>("SELECT * FROM Orden WHERE IdOrden = @ID", new { ID = orden.IdOrden })!;
    });
    public bool UpdEstadoPagado(int idOrden) => UseNewConnection(db =>
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdOrden", idOrden);
        db.Execute("PagarOrden", parameters);
        return true;
    });
    public bool UpdEstadoCancelado(int idOrden) => UseNewConnection(db =>
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdOrden", idOrden);
        db.Execute("CancelarOrden", parameters);
        return true;
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
    const string strData = @"SELECT T.TipoEntrada, O.Estado, O.Cierre 
                             FROM Orden O
                             JOIN Tarifa T USING (IdTarifa)
                             WHERE O.IdOrden = @ID";
    public bool UpdEstadoExpirado(int idOrden) => UseNewConnection(db => db.Execute(strUpdExpirado,new { ID = idOrden}) > 0);
    public bool UpdAbonado(int idOrden) => UseNewConnection(db => db.Execute(strUpdAbonado, new { ID = idOrden}) > 0);
    public bool UpdCancelado(int idOrden) => UseNewConnection(db => db.Execute(strUpdCancelado, new { ID = idOrden}) > 0);
    public bool Exists(int idOrden) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = idOrden}));
    public (ETipoEntrada, ETipoEstadoOrden, DateTime) Data(int unIdOrden) => UseNewConnection(db =>
    {
        return db.QueryFirst<(ETipoEntrada,ETipoEstadoOrden, DateTime)>(strData, new { ID = unIdOrden});
    });
    #endregion
}