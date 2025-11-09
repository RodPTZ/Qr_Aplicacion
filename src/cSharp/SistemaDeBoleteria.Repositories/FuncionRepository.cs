using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class FuncionRepository :  DbRepositoryBase, IFuncionRepository
{
    const string InsSql = @"INSERT INTO Funcion (IdEvento, IdSector, Apertura, Cierre) 
                            VALUES (@IdEvento, @IdSector, @Apertura, @Cierre);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Funcion 
                            SET IdSector = @IdSector,
                                Apertura = @Apertura,
                                Cierre = @Cierre
                                WHERE IdFuncion = @ID";
    const string UpdCancel = @"UPDATE Funcion 
                               SET Cancelado = true 
                               WHERE IdFuncion = @ID";

    public IEnumerable<Funcion> SelectAll() => db.Query<Funcion>("SELECT * FROM Funcion");
    
    public Funcion? Select(int idFuncion) => db.QueryFirstOrDefault<Funcion>("SELECT * FROM Funcion WHERE IdFuncion = @ID", new { ID = idFuncion });
    public Funcion Insert(Funcion funcion)
    {
        funcion.IdFuncion = db.ExecuteScalar<int>(InsSql, funcion);
        return Select(funcion.IdFuncion)!;
    }
    public Funcion Update(Funcion funcion, int IdFuncion)
    {
        db.Execute(UpdSql, new
        {
            funcion.IdSector,
            funcion.Apertura,
            funcion.Cierre,
            ID = IdFuncion
        });
        return Select(IdFuncion)!;
    }
    public void UpdFuncionCancel(int idFuncion) => db.Execute(UpdCancel, new { ID = idFuncion });
}
