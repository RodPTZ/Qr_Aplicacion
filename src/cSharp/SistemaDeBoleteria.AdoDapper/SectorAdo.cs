using SistemaDeBoleteria.Core.Services;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using System.Data;
using Mapster;

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
    
    public IEnumerable<MostrarSectorDTO> GetSectoresByLocalId(int idLocal)
    {
        var sql = "SELECT * FROM Sector WHERE IdLocal = @ID";
        return db.Query<MostrarSectorDTO>(sql, new { ID = idLocal });
    }
    public MostrarSectorDTO InsertSector(CrearActualizarSectorDTO sector, int idLocal)
    {

        var sql = "INSERT INTO Sector (IdLocal, Capacidad) VALUES (@IdLocal, @TipoSector); SELECT LAST_INSERT_ID()";
        var id = db.ExecuteScalar<int>(sql, new
        {
            idLocal,
            sector.Capacidad
        });
        var mostrarSector = sector.Adapt<MostrarSectorDTO>();
        mostrarSector.IdSector = id;
        return mostrarSector;
        
    }
    public MostrarSectorDTO UpdateSector(CrearActualizarSectorDTO sector, int idSector)
    {
        var sql = "UPDATE Sector SET Capacidad = @Capacidad WHERE IdSector = @ID";
        db.Execute(sql, new
        {
            sector.Capacidad,
            ID = idSector
        });
        var mostrarSector = sector.Adapt<MostrarSectorDTO>();
        return mostrarSector;
    }
    public bool DeleteSector(int id)
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
}
