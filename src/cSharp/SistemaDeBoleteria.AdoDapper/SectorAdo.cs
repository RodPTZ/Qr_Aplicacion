using SistemaDeBoleteria.Core.Services;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core;
using System.Data;

namespace SistemaDeBoleteria.AdoDapper;

public class SectorAdo : ISectorService
{
    private readonly IDbConnection db;
    public SectorAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public SectorAdo(IDbConnection dbConnection) => db = dbConnection;
    public SectorAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }
    
    public void InsertSector(Sector sector, int idLocal)
    {

        var sql = "INSERT INTO Sector (IdLocal, TipoSector) VALUES (@IdLocal, @TipoSector);";
        var id = db.ExecuteScalar<int>(sql, new
        {
            idLocal,
            sector.TipoSector
        });
        sector.IdSector = id;
    }

    public IEnumerable<Sector> GetSectoresByLocalId(int idLocal)
    {
        var sql = "SELECT * FROM Sector WHERE IdLocal = @ID";
        return db.Query<Sector>(sql, new { ID = idLocal });
    }
    public Sector? GetSectorByLocalId(int idLocal)
    {
        var sql = "SELECT * FROM Sector WHERE IdSector = @ID";
        return db.QueryFirstOrDefault<Sector>(sql, new { ID = idLocal });
    }
    public bool UpdateSector(Sector sector, int id)
    {
        var sql = "UPDATE Sector SET TipoSector = @TipoSector WHERE IdSector = @ID";
        db.Execute(sql, new
        {
            sector.TipoSector,
            ID = id
        });
        return true;
    }
    public bool DeleteSector(int id)
    {
        var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdSector = @ID";
        var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
        if (cantidadFunciones == 0)
        {
            var sql = "DELETE FROM Sector WHERE IdSector = @ID";
            db.Execute(sql, new { ID = id });
            return true;
        }
        return false;
    }
    public Sector GetSectorById(int idSector)
    {
        var sql = "SELECT * FROM Sector WHERE IdSector = @ID";
        return db.QueryFirstOrDefault<Sector>(sql, new { ID = idSector })!;
    }
}
