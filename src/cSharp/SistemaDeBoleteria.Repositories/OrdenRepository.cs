using SistemaDeBoleteria.Core.Models;
using Dapper;
using MySqlConnector;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using System.Data;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Inheritance;
namespace SistemaDeBoleteria.Repositories;

public class OrdenRepository :  DbRepositoryBase, IOrdenRepository
{
    public IEnumerable<Orden> SelectAll() => db.Query<Orden>("SELECT * FROM Orden");
    public Orden? Select(int idOrden) => db.QueryFirstOrDefault<Orden>("SELECT * FROM Orden WHERE IdOrden = @ID", new { ID = idOrden });
    
    public Orden Insert(Orden orden)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdOrden", direction: ParameterDirection.Output);
        parameters.Add("@unIdTarifa", orden.IdTarifa);
        parameters.Add("@unIdCliente", orden.IdCliente);
        parameters.Add("@unIdFuncion", orden.IdFuncion);
        parameters.Add("@unMedioDePago", orden.MedioDePago);
        try
        {
            db.Execute("AltaOrden", parameters);
            orden.IdOrden = parameters.Get<int>("@unIdOrden");
            return Select(orden.IdOrden)!;
        }   
        catch( MySqlException ex)
        {
            throw new ConstraintException(ex.Message);
        }
        // orden.IdOrden = db.ExecuteScalar<int>(InsSql, new
        // {
        //     orden.IdCliente,
        //     orden.IdFuncion,
        //     TipoEntrada = orden.tipoEntrada.ToString(),
        //     orden.MedioDePago
        // }); 
        // return Select(orden.IdOrden)!;
    }
    public bool UpdEstadoPagado(int idOrden)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdOrden", idOrden);
        try
        {
            db.Execute("PagarOrden", parameters);
            return true;
        }
        catch(MySqlException ex)
        {
            throw new ConstraintException(ex.Message);
        }
    }
    public bool UpdEstadoCancelado(int idOrden)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@unIdOrden", idOrden);
        try
        {
            db.Execute("CancelarOrden", parameters);
            return true;
        }
        catch (MySqlException ex)
        {
            throw new ConstraintException(ex.Message);
        }
        
    }
    
}
