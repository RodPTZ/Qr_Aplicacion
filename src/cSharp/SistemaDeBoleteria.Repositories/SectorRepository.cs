using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Models;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class SectorRepository :  DbRepositoryBase, ISectorRepository
{
    public SectorRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Sector (IdLocal, Capacidad) 
                            VALUES (@IdLocal, @Capacidad); 
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Sector 
                            SET Capacidad = @Capacidad 
                            WHERE IdSector = @ID";
    public IEnumerable<Sector> SelectAllByLocalId(int idLocal) => UseNewConnection(db => db.Query<Sector>("SELECT * FROM Sector WHERE IdLocal = @ID", new { ID = idLocal }));

    public Sector? Select(int idSector) => UseNewConnection(db => db.QueryFirstOrDefault<Sector>("SELECT * FROM Sector WHERE IdSector = @ID", new { ID = idSector}));
    
    public Sector Insert(Sector sector, int idLocal) => UseNewConnection(db=>
    {
        sector.IdLocal = idLocal;
        sector.IdSector = db.ExecuteScalar<int>(InsSql, new
        {
            sector.IdLocal,
            sector.Capacidad
        });
        return sector;
    });
    public bool Update(Sector sector, int idSector) => UseNewConnection(db => db.Execute(UpdSql, new{ sector.Capacidad, ID = idSector }) > 0);
    
    public bool Delete(int id) => UseNewConnection(db => db.Execute("DELETE FROM Sector WHERE IdSector = @ID", new { ID = id }) > 0);

    #region Validación de negocio
    const string strHasFunciones = @"SELECT EXISTS (SELECT 1 
                                                    FROM Funcion 
                                                    WHERE IdSector = @ID)";
    const string strExists = @"SELECT EXISTS (SELECT 1 
                                              FROM Sector 
                                              WHERE IdSector = @ID)";
    public bool HasFunciones(int idSector) => UseNewConnection(db => db.ExecuteScalar<bool>(strHasFunciones, new { ID = idSector }));
    public bool Exists(int idSector) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idSector }));
    #endregion
}
