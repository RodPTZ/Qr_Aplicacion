using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;
using System.Data.Common;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Repositories;

public class EntradaRepository :  DbRepositoryBase, IEntradaRepository
{
    public EntradaRepository(string connectionString) : base (connectionString){}

    const string InsSql = @"INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
                            VALUES (@IdOrden, @unTipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

                            SELECT LAST_INSERT_ID();";
    const string UpdCancel = @"UPDATE Entrada
                               SET Anulado = TRUE
                               WHERE IdEntrada = @ID;";
    public IEnumerable<Entrada> SelectAll() => UseNewConnection(db => db.Query<Entrada>("SELECT * FROM Entrada"));
    public Entrada? Select(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<Entrada>("SELECT * FROM Entrada WHERE IdEntrada = @ID", new { ID = idEntrada }));

    public int Insert(int idOrden, ETipoEntrada tipoDeEntrada) => UseNewConnection(db =>
    {
        var idEntrada = db.ExecuteScalar<int>(InsSql, new { IdOrden = idOrden, unTipoEntrada = tipoDeEntrada});
        return Exists(idEntrada) ? idEntrada : 0 ;
}   );

    public bool UpdAnular(int idEntrada) => UseNewConnection(db => db.Execute(UpdCancel, new{ ID = idEntrada}) > 0);
    
    #region Validación de negocio
    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Entrada 
                                             WHERE IdEntrada = @ID)";
    const string strObtenerFuncion = @"SELECT F.IdFuncion
                                       FROM Funcion F
                                       JOIN Orden O ON F.IdSesion = O.IdSesion
                                       JOIN Entrada E ON E.IdOrden = O.IdOrden
                                       WHERE E.IdEntrada = @ID;";
    const string strAnularEntradasEvento = @"UPDATE Entrada
                                       SET Anulado = TRUE
                                       WHERE IdOrden IN (SELECT IdOrden 
                                                        FROM Orden O 
                                                        JOIN Tarifa T USING (IdTarifa)
                                                        JOIN Funcion F USING (IdFuncion)
                                                        WHERE F.IdEvento = @ID);";
    const string strAnularEntradasFuncion = @"UPDATE Entrada
                                       SET Anulado = TRUE
                                       WHERE IdOrden IN (SELECT IdOrden 
                                                        FROM Orden O 
                                                        JOIN Tarifa T USING (IdTarifa) 
                                                        WHERE T.IdFuncion = @ID);";
    public bool Exists(int idEntrada) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idEntrada}));
    public int ObtenerFuncion(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<int>(strObtenerFuncion, new { ID = idEntrada}));

    public bool UpdAnularEntradasDeEventoID(int idEvento) => UseNewConnection( db => db.Execute(strAnularEntradasEvento, new { ID = idEvento}) > 0);
    public bool UpdAnularEntradasDeFuncionID(int idFuncion) => UseNewConnection( db => db.Execute(strAnularEntradasFuncion, new { ID = idFuncion}) > 0);
    #endregion
}
