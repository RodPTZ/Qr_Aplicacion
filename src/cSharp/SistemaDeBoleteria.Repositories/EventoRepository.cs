using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class EventoRepository :  DbRepositoryBase, IEventoRepository
{
    public MostrarEventoDTO? GetEventoById(int id)
    {
        var sql = "SELECT * FROM Evento WHERE IdEvento = @ID";
        return db.QueryFirstOrDefault<MostrarEventoDTO>(sql, new { ID = id });
    }

    public MostrarEventoDTO InsertEvento(CrearActualizarEventoDTO evento)
    {
        var sql = "INSERT INTO Evento (IdLocal, Nombre, Tipo, publicado) VALUES (@IdLocal, @Nombre, @Tipo, @Publicado); SELECT LAST_INSERT_ID();";
        var id = db.ExecuteScalar<int>(sql, evento);
        var mostrarEvento = evento.Adapt<MostrarEventoDTO>();
        mostrarEvento.IdEvento = id;
        return mostrarEvento;
    }
    public MostrarEventoDTO UpdateEvento(CrearActualizarEventoDTO evento, int IdEvento)
    {
        var sql = "UPDATE Evento SET IdLocal = @IdLocal, Nombre = @Nombre, Tipo = @Tipo WHERE IdEvento = @ID";
        db.Execute(sql, new
        {
            evento.IdLocal,
            evento.Nombre,
            Tipo = evento.Tipo.ToString(),
            ID = IdEvento
        });
        var eventoActualizado = evento.Adapt<MostrarEventoDTO>();
        eventoActualizado.IdEvento = IdEvento;
        return eventoActualizado;
    }
    public bool PublicarEvento(int id)
    {
        var consulta = "SELECT COUNT(*) FROM Funcion WHERE IdEvento = @ID";
        var cantidadFunciones = db.ExecuteScalar<int>(consulta, new { ID = id });
        if (cantidadFunciones == 0)
            return false;
        var sql = "UPDATE Evento SET publicado = true WHERE IdEvento = @ID";
        db.Execute(sql, new { ID = id });
        return true;
    }
    public bool CancelarEvento(int id)
    {
        var sql = "UPDATE Evento SET publicado = false WHERE IdEvento = @ID";
        db.Execute(sql, new { ID = id });
        return true;
    }
}
