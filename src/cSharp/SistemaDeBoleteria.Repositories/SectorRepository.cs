using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Models;
using System.Data;
using Mapster;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class SectorRepository :  DbRepositoryBase, ISectorRepository
{
    const string InsSql = @"INSERT INTO Sector (IdLocal, Capacidad) 
                            VALUES (@IdLocal, @Capacidad); 
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Sector 
                            SET Capacidad = @Capacidad 
                            WHERE IdSector = @ID";
    public IEnumerable<Sector> SelectAllByLocalId(int idLocal) => db.Query<Sector>("SELECT * FROM Sector WHERE IdLocal = @ID", new { ID = idLocal });
    
    public Sector Insert(Sector sector, int idLocal)
    {
        sector.IdLocal = idLocal;
        sector.IdSector = db.ExecuteScalar<int>(InsSql, new
        {
            sector.IdLocal,
            sector.Capacidad
        });
        return sector;
        
    }
    public Sector Update(Sector sector, int idSector)
    {
        db.Execute(UpdSql, new
        {
            sector.Capacidad,
            ID = idSector
        });
        return Select(idSector)!;
    }
    public bool Delete(int id)
    {
        var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdSector = @ID";
        var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
        if (cantidadFunciones == 0)
        {
            var sql = "DELETE FROM Sector WHERE IdSector = @ID";
            db.Execute(sql, new { ID = id });
            return true;        // Se espera retornar una salida de la bd
        }
        return false;
    }
    public Sector? Select(int idSector) => db.QueryFirstOrDefault<Sector>("SELECT * FROM Sector WHERE IdSector = @ID", new { ID = idSector});
}
