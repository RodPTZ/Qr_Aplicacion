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
    public TarifaRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Tarifa (IdFuncion, TipoEntrada, Precio, Stock) 
                            VALUES (@IdFuncion, @TipoEntrada, @Precio, @Stock);
                            
                            SELECT LAST_INSERT_ID()";
    const string UpdSql = @"UPDATE Tarifa 
                            SET Precio = @Precio, 
                                Stock = @Stock, 
                                Estado = @Estado 
                            WHERE IdTarifa = @ID";
    public IEnumerable<Tarifa> SelectAllByFuncionId(int idFuncion) => UseNewConnection(db => db.Query<Tarifa>("SELECT * FROM Tarifa WHERE IdFuncion = @ID", new { ID = idFuncion }));

    public Tarifa? Select(int IdTarifa) => UseNewConnection(db => db.QueryFirstOrDefault<Tarifa>("SELECT * FROM Tarifa WHERE IdTarifa = @ID", new { ID = IdTarifa }));
    
    public Tarifa Insert(Tarifa tarifa) => UseNewConnection(db =>
    {
        tarifa.IdTarifa = db.ExecuteScalar<int>(InsSql, tarifa);
        return Select(tarifa.IdTarifa)!;
    });
    public bool Update(Tarifa tarifa, int IdTarifa) => UseNewConnection(db =>
    {
        return db.Execute(UpdSql, new
        {
            tarifa.Precio,
            tarifa.Stock,
            tarifa.Estado,
            ID = IdTarifa
        }) > 0;
    });

    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Tarifa 
                                             WHERE IdTarifa = @ID)";
    public bool Exists(int idTarifa) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new{ ID = idTarifa }));
        
}
