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
                               SET Estado = 'Anulado'
                               WHERE IdEntrada = @ID;";
    public IEnumerable<Entrada> SelectAll() => UseNewConnection(db => db.Query<Entrada>("SELECT * FROM Entrada"));
    public Entrada? Select(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<Entrada>("SELECT * FROM Entrada WHERE IdEntrada = @ID", new { ID = idEntrada }));

    public int Insert(int idOrden, ETipoEntrada tipoDeEntrada) => UseNewConnection(db =>
    {
        var idEntrada = db.ExecuteScalar<int>(InsSql, new { IdOrden = idOrden, unTipoEntrada = tipoDeEntrada});
        return Exists(idEntrada) ? idEntrada : 0 ;
}   );

    public bool UpdateEstado(int idEntrada) => UseNewConnection(db =>
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdEntrada", idEntrada);
        db.Execute("CancelarEntrada", parameters);
        return true;
    });
    public bool UpdAnular(int idEntrada) => UseNewConnection(db => db.Execute(UpdCancel, new{ ID = idEntrada}) > 0);
    
    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Entrada 
                                             WHERE IdEntrada = @ID)";
    public bool Exists(int idEntrada) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idEntrada}));
    const string strObtenerFuncion = @"SELECT F.IdFuncion
                                       FROM Funcion F
                                       JOIN Orden O ON F.IdSesion = O.IdSesion
                                       JOIN Entrada E ON E.IdOrden = O.IdOrden
                                       WHERE E.IdEntrada = @ID;";
    public int ObtenerFuncion(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<int>(strObtenerFuncion, new { ID = idEntrada}));

    const string strAnularEntradas = @"UPDATE Entrada
                                       SET Estado = 'Anulado'
                                       WHERE IdOrden IN (SELECT IdOrden 
                                                        FROM Orden O 
                                                        JOIN Funcion F USING (IdFuncion) 
                                                        WHERE F.IdEvento = @ID);";
    public bool UpdAnularEntradasDeEventoID(int idEvento) => UseNewConnection( db => db.Execute(strAnularEntradas, new { ID = idEvento}) > 0);

}
