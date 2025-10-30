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
    const string InsSql = @"INSERT INTO Evento (IdLocal, Nombre, Tipo, publicado) 
                            VALUES (@IdLocal, @Nombre, @Tipo, @Publicado); 
                            
                            SELECT LAST_INSERT_ID();";
    const string UpdSql = @"UPDATE Evento 
                            SET IdLocal = @IdLocal, 
                                Nombre = @Nombre, 
                                Tipo = @Tipo 
                            WHERE IdEvento = @ID";
    public IEnumerable<Evento> SelectAll() => db.Query<Evento>("SELECT * FROM Evento");
    public Evento? Select(int IdEvento) => db.QueryFirstOrDefault<Evento>("SELECT * FROM Evento WHERE IdEvento = @ID", new { ID = IdEvento });
    public Evento Insert(Evento evento)
    {
        evento.IdEvento = db.ExecuteScalar<int>(InsSql, evento);
        return Select(evento.IdEvento)!;
    }
    public Evento Update(Evento evento, int IdEvento)
    {
        db.Execute(UpdSql, new
        {
            evento.IdLocal,
            evento.Nombre,
            Tipo = evento.Tipo.ToString(),
            ID = IdEvento
        });
        evento.IdEvento = IdEvento;
        return evento;
    }
    public (byte secuencia, string? ErrorMessage) UpdEstadoPublic(int IdEvento)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ID", IdEvento);
        try
        {
            db.Execute("PublicarEvento", parameters);
            return (1, "Evento publicado exitosamente.");
        }
        catch (MySqlException ex)
        {
            return (2, ex.Message);
        }
    }
    public bool UpdEstadoCancel(int IdEvento)
    {
        var sql = "UPDATE Evento SET publicado = false WHERE IdEvento = @ID";
        db.Execute(sql, new { ID = IdEvento });
        return true;
    }
}
