using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using System.Reflection.Metadata.Ecma335;
using System.Data.Common;
namespace SistemaDeBoleteria.Repositories;
public class TarifaRepository :  DbRepositoryBase, ITarifaRepository
{
    public TarifaRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Tarifa (IdFuncion, TipoEntrada, Precio, Stock) 
                            VALUES (@IdFuncion, @TipoEntrada, @Precio, @Stock);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Tarifa 
                            SET Precio = @Precio, 
                                Stock = @Stock, 
                                Estado = @Estado 
                            WHERE IdTarifa = @ID";
    public IEnumerable<Tarifa> SelectAllByFuncionId(int idFuncion) => UseNewConnection(db => db.Query<Tarifa>("SELECT * FROM Tarifa WHERE IdFuncion = @ID", new { ID = idFuncion }));

    public Tarifa? Select(int IdTarifa) => UseNewConnection(db => db.QueryFirstOrDefault<Tarifa>("SELECT * FROM Tarifa WHERE IdTarifa = @ID", new { ID = IdTarifa }));
    
    public Tarifa Insert(Tarifa tarifa) => UseNewConnection(db =>
    {
        tarifa.IdTarifa = db.ExecuteScalar<int>(InsSql, tarifa);
        return Select(tarifa.IdTarifa)!;
    });
    public bool Update(Tarifa tarifa, int IdTarifa) => UseNewConnection(db =>
    {
        return db.Execute(UpdSql, new
        {
            tarifa.Precio,
            tarifa.Stock,
            tarifa.Estado,
            ID = IdTarifa
        }) > 0;
    });

    #region Validación de negocio
    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Tarifa 
                                             WHERE IdTarifa = @ID)";
    const string strUpdReducirStock = @"UPDATE Tarifa
                                        SET Stock = Stock - 1
                                        WHERE IdTarifa = @unIdTarifa;";
    const string strDevolverReservaStock = @"UPDATE Tarifa
                                             SET Stock = Stock + 1 
                                             WHERE IdTarifa = (SELECT IdTarifa
                                                                FROM Orden
                                                                WHERE IdOrden = @ID);";
    const string strSuspenderTarifasEvento = @"UPDATE Tarifa
                                               SET Estado = 'Suspendida'
                                               WHERE IdFuncion IN (SELECT IdFuncion 
                                                                   FROM Funcion 
                                                                   WHERE IdEvento = @ID);
                                               UPDATE Tarifa
                                               SET Stock = Stock + (SELECT COUNT(*)
                                                                    FROM Entrada E
                                                                    JOIN Orden O USING (IdOrden)
                                                                    WHERE E.Anulado = TRUE
                                                                    AND O.IdTarifa = Tarifa.IdTarifa)
                                               WHERE IdFuncion IN (SELECT IdFuncion 
                                                                   FROM Funcion 
                                                                   WHERE IdEvento = @ID);";
    const string strSuspenderTarifasFuncion = @"UPDATE Tarifa
                                                SET Estado = 'Suspendida'
                                                WHERE IdFuncion = @ID;

                                                UPDATE Tarifa
                                                SET Stock = Stock + (SELECT COUNT(*) 
                                                                     FROM Entrada E
                                                                     JOIN Orden O USING (IdOrden)
                                                                     WHERE E.Anulado = TRUE
                                                                     AND O.IdTarifa = Tarifa.IdTarifa)
                                                WHERE IdFuncion = @ID";
    public bool Exists(int idTarifa) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = idTarifa }));
    public bool DevolverStock(int idOrden) 
    => UseNewConnection(db =>
                db.Execute(strDevolverReservaStock, new { ID = idOrden}) > 0
        );
    public bool ReducirStock(int idTarifa) 
    => UseNewConnection(db =>
                db.Execute(strUpdReducirStock, new { unIdTarifa = idTarifa}) > 0
        );                                                        
    public bool SuspenderTarifasPorIdEvento(int idEvento) => UseNewConnection(db => db.Execute(strSuspenderTarifasEvento, new { ID = idEvento})> 0);
    public bool SuspenderTarifasPorIdFuncion(int idFuncion) => UseNewConnection(db => db.Execute(strSuspenderTarifasFuncion, new { ID = idFuncion})> 0);
    #endregion
}
