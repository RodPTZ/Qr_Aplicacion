using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using Mapster;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class FuncionRepository :  DbRepositoryBase, IFuncionRepository
{
    public MostrarFuncionDTO InsertFuncion(CrearFuncionDTO funcion)
    {
        var sql = "INSERT INTO Funcion (IdEvento, IdSector, IdSesion,  Fecha, Duracion) VALUES (@IdEvento, @IdSector, @IdSesion, @Duracion, @Fecha);";
        var id = db.ExecuteScalar<int>(sql, funcion);
        var mostrarFuncion = funcion.Adapt<MostrarFuncionDTO>();
        mostrarFuncion.IdFuncion = id;
        return mostrarFuncion;
    }

    public IEnumerable<MostrarFuncionDTO> GetFunciones()
    {
        var sql = "SELECT * FROM Funcion";
        return db.Query<MostrarFuncionDTO>(sql);
    }

    public MostrarFuncionDTO? GetFuncionById(int IdFuncion)
    {
        var sql = "SELECT * FROM Funcion WHERE IdFuncion = @ID";
        return db.QueryFirstOrDefault<MostrarFuncionDTO>(sql, new { ID = IdFuncion });
    }

    public MostrarFuncionDTO UpdateFuncion(ActualizarFuncionDTO funcion, int IdFuncion)
    {
        var sql = "UPDATE Funcion SET IdSector = @IdSector, IdSesion = @IdSesion, Fecha = @Fecha, Duracion = @Duracion WHERE IdFuncion = @ID";
        db.Execute(sql, new
        {
            funcion.IdSector,
            funcion.IdSesion,
            funcion.Fecha,
            funcion.Duracion,
            ID = IdFuncion
        });
        var funcionActualizada = funcion.Adapt<MostrarFuncionDTO>();
        funcionActualizada.IdFuncion = IdFuncion;
        return funcionActualizada;
    }
    public void CancelarFuncion(int IdFuncion)
    {
        var sql = "UPDATE Funcion SET cancelado = true WHERE IdFuncion = @ID";
        db.Execute(sql, new { ID = IdFuncion });
    }

}
