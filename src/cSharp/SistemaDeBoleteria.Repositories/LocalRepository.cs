using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using Mapster;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class LocalRepository :  DbRepositoryBase, ILocalRepository
{
    const string InsSql = @"INSERT INTO Local (Nombre, Ubicacion) 
                            VALUES (@Nombre, @Ubicacion);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Local 
                            SET Nombre = @Nombre,
                                Ubicacion = @Ubicacion 
                            WHERE IdLocal = @ID";
    const string DelSql = @"SELECT COUNT(*) 
                            FROM Funcion 
                            JOIN Sector USING (IdSector) 
                            WHERE IdLocal = @ID";
    public IEnumerable<Local> SelectAll() => db.Query<Local>("SELECT * FROM Local");
    
    public Local? Select(int id) => db.QueryFirstOrDefault<Local>("SELECT * FROM Local WHERE IdLocal = @ID", new { ID = id });

    public Local Insert(Local local)
    {
        local.IdLocal = db.ExecuteScalar<int>(InsSql, local);
        return local;
    }
    public Local Update(Local local, int IdLocal)
    {
        db.Execute(UpdSql, new
        {
            local.Nombre,
            local.Ubicacion,
            ID = IdLocal
        });
        return Select(IdLocal)!;
    }
    public bool Delete(int IdLocal)
    {
        var CantFunciones = db.ExecuteScalar<int>(DelSql, new { ID = IdLocal });
        if (CantFunciones != 0)
            return false;
        // falta verificar también que no haya un evento asociado al Local.
        db.Execute("DELETE FROM Local WHERE IdLocal = @ID", new { ID = IdLocal });
        return true;
    }
}
