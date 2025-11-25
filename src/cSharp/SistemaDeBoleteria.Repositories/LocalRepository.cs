using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class LocalRepository :  DbRepositoryBase, ILocalRepository
{
    public LocalRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Local (Nombre, Ubicacion) 
                            VALUES (@Nombre, @Ubicacion);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Local 
                            SET Nombre = @Nombre,
                                Ubicacion = @Ubicacion 
                            WHERE IdLocal = @ID";
    public IEnumerable<Local> SelectAll() => UseNewConnection(db => db.Query<Local>("SELECT * FROM Local"));
    
    public Local? Select(int id) => UseNewConnection(db => db.QueryFirstOrDefault<Local>("SELECT * FROM Local WHERE IdLocal = @ID", new { ID = id }));
    public Local Insert(Local local) => UseNewConnection(db =>
    {
        local.IdLocal = db.ExecuteScalar<int>(InsSql, local);
        return local;
    });
    public bool Update(Local local, int IdLocal) => UseNewConnection(db => db.Execute(UpdSql, new{ local.Nombre, local.Ubicacion, ID = IdLocal}) > 0);
    
    public bool Delete(int IdLocal) => UseNewConnection(db => db.Execute("DELETE FROM Local WHERE IdLocal = @ID", new { ID = IdLocal }) > 0);

    #region  Validación de negocio
    
    const string strHasEventos = @"SELECT EXISTS (SELECT 1 
                                                  FROM Evento 
                                                  WHERE IdLocal = @ID)";
    const string strHasFunciones = @"SELECT EXISTS (SELECT 1 
                                                    FROM Funcion 
                                                    JOIN Sector USING (IdSector)
                                                    WHERE IdLocal = @ID)";
    const string strExists = @"SELECT EXISTS (SELECT 1 
                                              FROM Local 
                                              WHERE IdLocal = @ID)";
    public bool HasEventos(int idLocal) => UseNewConnection(db => db.ExecuteScalar<bool>(strHasEventos, new { ID = idLocal }));
    public bool HasFunciones(int idLocal) => UseNewConnection(db => db.ExecuteScalar<bool>(strHasFunciones, new { ID = idLocal }));
    public bool Exists(int idLocal) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idLocal }));

    #endregion
}
