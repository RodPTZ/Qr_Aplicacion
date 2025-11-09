using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using System.Reflection.Metadata.Ecma335;
namespace SistemaDeBoleteria.Repositories;
public class TarifaRepository :  DbRepositoryBase, ITarifaRepository
{
    const string InsSql = @"INSERT INTO Tarifa (IdFuncion, TipoEntrada, Precio, Stock) 
                            VALUES (@IdFuncion, @TipoEntrada, @Precio, @Stock);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Tarifa 
                            SET Precio = @Precio, 
                                Stock = @Stock, 
                                Estado = @Estado 
                            WHERE IdTarifa = @ID";
    public IEnumerable<Tarifa> SelectAllByFuncionId(int idFuncion) => db.Query<Tarifa>("SELECT * FROM Tarifa WHERE IdFuncion = @ID", new { ID = idFuncion });

    public Tarifa? Select(int IdTarifa) => db.QueryFirstOrDefault<Tarifa>("SELECT * FROM Tarifa WHERE IdTarifa = @ID", new { ID = IdTarifa });
    
    public Tarifa Insert(Tarifa tarifa)
    {
        tarifa.IdTarifa = db.ExecuteScalar<int>(InsSql, tarifa);
        return Select(tarifa.IdTarifa)!;
    }   
    public Tarifa Update(Tarifa tarifa, int IdTarifa)
    {
        db.Execute(UpdSql, new
        {
            tarifa.Precio,
            tarifa.Stock,
            tarifa.Estado,
            ID = IdTarifa
        });
        return Select(IdTarifa)!;
    }
        
}
