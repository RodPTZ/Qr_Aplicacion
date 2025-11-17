using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Inheritance;
using System.Data.Common;

namespace SistemaDeBoleteria.Repositories;

public class EntradaRepository :  DbRepositoryBase, IEntradaRepository
{
    // public EntradaRepository(string connectionString) : base (connectionString){}
    public IEnumerable<Entrada> SelectAll() => UseNewConnection(db => db.Query<Entrada>("SELECT * FROM Entrada"));
    public Entrada? Select(int idEntrada) => UseNewConnection(db => db.QueryFirstOrDefault<Entrada>("SELECT * FROM Entrada WHERE IdEntrada = @ID", new { ID = idEntrada }));
    
    public bool UpdateEstado(int idEntrada) => UseNewConnection(db =>
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdEntrada", idEntrada);
        db.Execute("CancelarEntrada", parameters);
        return true;
    });
    const string strExists = @"SELECT EXITS(SELECT 1 
                                            FROM Entrada 
                                            WHERE IdEntrada = @ID)";
    public bool Exists(int idEntrada) => UseNewConnection(db => db.ExecuteScalar<bool>(strExists, new { ID = idEntrada}));
}
