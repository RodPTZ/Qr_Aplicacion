using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeBoleteria.Repositories;

public class FuncionRepository :  DbRepositoryBase, IFuncionRepository
{
    // public FuncionRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Funcion (IdEvento, IdSector, Apertura, Cierre) 
                            VALUES (@IdEvento, @IdSector, @Apertura, @Cierre);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Funcion 
                            SET IdSector = @IdSector,
                                Apertura = @Apertura,
                                Cierre = @Cierre
                                WHERE IdFuncion = @ID";

    public IEnumerable<Funcion> SelectAll() => UseNewConnection(db => db.Query<Funcion>("SELECT * FROM Funcion"));

    public Funcion? Select(int idFuncion) => UseNewConnection(db =>
    {
        var funcion = db.QueryFirstOrDefault<Funcion>("SELECT * FROM Funcion WHERE IdFuncion = @ID", new { ID = idFuncion });
        if (funcion is null)
            return null;

        funcion.Fecha = DateOnly.FromDateTime(funcion.Apertura);
        funcion.AperturaTime = TimeOnly.FromDateTime(funcion.Apertura);
        funcion.CierreTime = TimeOnly.FromDateTime(funcion.Cierre);
        
        return funcion;
    });
    public Funcion Insert(Funcion funcion) => UseNewConnection(db =>
    {
        funcion.IdFuncion = db.ExecuteScalar<int>(InsSql, new
        {
            funcion.IdEvento,
            funcion.IdSector,
            Apertura = funcion.Fecha.ToDateTime(funcion.AperturaTime),
            Cierre = funcion.Fecha.ToDateTime(funcion.CierreTime)
        });
        return Select(funcion.IdFuncion)!;
    });
    public bool Update(Funcion funcion, int IdFuncion) => UseNewConnection(db =>
    {
        return db.Execute(UpdSql, new
        {
            funcion.IdSector,
            Apertura = funcion.Fecha.ToDateTime(funcion.AperturaTime),
            Cierre = funcion.Fecha.ToDateTime(funcion.CierreTime),
            ID = IdFuncion
        }) > 0;
    });
    public bool UpdFuncionCancel(int idFuncion) => UseNewConnection(db =>
    { 
        var parameters = new DynamicParameters();
        parameters.Add(@"unIdFuncion", idFuncion);
        db.Execute("CancelarFuncion", parameters);
        return true;
    });

    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Funcion 
                                             WHERE IdFuncion = @ID)";
    public bool Exists(int idFuncion) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = idFuncion }));

}
