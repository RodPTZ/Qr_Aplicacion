using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Models;
using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class EntradaRepository :  DbRepositoryBase, IEntradaRepository
{
    public IEnumerable<Entrada> SelectAll() => db.Query<Entrada>("SELECT * FROM Entrada");
    public Entrada? Select(int idEntrada) => db.QueryFirstOrDefault<Entrada>("SELECT * FROM Entrada WHERE IdEntrada = @ID", new { ID = idEntrada });
    
    public bool UpdateEstado(int idEntrada)
    {
        // var sql = "UPDATE Entrada SET Anulada = true WHERE IdEntrada = @ID";
        // db.Execute(sql, new { ID = idEntrada });
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@unIdEntrada", idEntrada);
            db.Execute("CancelarEntrada", parameters);
            return true;
        }catch(MySqlException ex)
        {
            throw new ConstraintException(ex.Message);
        }
    }
}
