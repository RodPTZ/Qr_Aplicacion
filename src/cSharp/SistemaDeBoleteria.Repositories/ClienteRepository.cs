using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Inheritance;

namespace SistemaDeBoleteria.Repositories;

public class ClienteRepository : DbRepositoryBase, IClienteRepository
{
    public ClienteRepository(string connectionString) : base (connectionString){}
    const string InsSql = @"INSERT INTO Cliente (IdUsuario, Nombre, Apellido, DNI, Telefono, Localidad, Edad) 
                            VALUES (@IdUsuario, @Nombre, @Apellido, @DNI, @Telefono, @Localidad, @Edad); 

                            SELECT LAST_INSERT_ID();";
    const string UpdSql = @"UPDATE Cliente 
                            SET Nombre = @Nombre, 
                                Apellido = @Apellido,
                                Telefono = @Telefono,
                                Localidad = @Localidad
                            WHERE IdCliente = @IdCliente;";
    public IEnumerable<Cliente> SelectAll() => UseNewConnection(db => db.Query<Cliente>("SELECT * FROM Cliente"));
    public Cliente? Select(int idCliente) => UseNewConnection(db => db.QueryFirstOrDefault<Cliente>("SELECT * FROM Cliente WHERE IdCliente = @ID", new { ID = idCliente }));
    public Cliente Insert(Cliente cliente) => UseNewConnection(db =>
    {
        cliente.IdCliente = db.ExecuteScalar<int>(InsSql, cliente);
        return Select(cliente.IdCliente)!;
    });
    public bool Update(Cliente cliente, int idCliente) => UseNewConnection(db =>
    {
        cliente.IdCliente = idCliente;
        return db.Execute(UpdSql, cliente) > 0;
    });
    
    const string strExists = @"SELECT EXISTS(SELECT 1 
                                             FROM Cliente 
                                             WHERE IdCliente = @ID)";
    public bool Exists(int idCliente) => UseNewConnection(db => db.ExecuteScalar<bool>( strExists, new{ ID = idCliente })); 

}
