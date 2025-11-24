using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class EventoRepository :  DbRepositoryBase, IEventoRepository
{
    public EventoRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Evento (IdLocal, Nombre, Tipo) 
                            VALUES (@IdLocal, @Nombre, @Tipo); 
                            
                            SELECT LAST_INSERT_ID();";
    const string UpdSql = @"UPDATE Evento 
                            SET IdLocal = @IdLocal, 
                                Nombre = @Nombre, 
                                Tipo = @Tipo 
                            WHERE IdEvento = @ID";
    public IEnumerable<Evento> SelectAll() => UseNewConnection(db => db.Query<Evento>("SELECT * FROM Evento"));
    public Evento? Select(int IdEvento) => UseNewConnection(db => db.QueryFirstOrDefault<Evento>("SELECT * FROM Evento WHERE IdEvento = @ID", new { ID = IdEvento }));
    public Evento Insert(Evento evento) => UseNewConnection(db =>
    {
        evento.IdEvento = db.ExecuteScalar<int>(InsSql, evento);
        return Select(evento.IdEvento)!;
    });
    public bool Update(Evento evento, int IdEvento) => UseNewConnection(db=>
    {
        return db.Execute(UpdSql, new
        {
            evento.IdLocal,
            evento.Nombre,
            evento.Tipo,
            ID = IdEvento
        }) > 0;
    });
    #region Validación de negocio
    
    const string strExists = @"SELECT EXISTS  (SELECT 1 
                                                FROM Evento WHERE IdEvento = @ID)";
    const string strHasFunciones = @"SELECT EXISTS(SELECT 1 
                                                   FROM Funcion 
                                                   WHERE IdEvento = @ID)";
    const string strHasTarifasActivas = @"SELECT EXISTS(SELECT 1 
                                                        FROM Tarifa T 
                                                        JOIN Funcion F ON F.IdFuncion = T.IdFuncion 
                                                        WHERE F.IdEvento = @ID 
                                                        AND T.Estado = 'Activa')";
    const string srtUpdPublicado = @"UPDATE Evento
                                     SET Estado = 'Publicado'
                                     WHERE IdEvento = @ID;";
    
    const string strUpdCancelado = @"UPDATE Evento
                                     SET Estado = 'Cancelado'
                                     WHERE IdEvento = @ID;";
    public bool Exists(int IdEvento) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = IdEvento }));
    public bool HasFunciones(int idEvento) => UseNewConnection(db => db.ExecuteScalar<bool>(strHasFunciones, new { ID = idEvento}));
    public bool HasTarifasActivas(int idEvento) => UseNewConnection(db => db.ExecuteScalar<bool>(strHasTarifasActivas, new {ID = idEvento}));
    public bool UpdPublicado(int idEvento) => UseNewConnection( db => db.Execute(srtUpdPublicado, new { ID = idEvento}) > 0);
    public bool UpdCancelar(int idEvento) => UseNewConnection(db => db.Execute(strUpdCancelado, new { ID = idEvento }) > 0 );
    
    #endregion
}
