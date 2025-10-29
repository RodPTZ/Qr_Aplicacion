using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Inheritance;
using Mapster;

namespace SistemaDeBoleteria.Repositories;

public class ClienteRepository : DbRepositoryBase, IClienteRepository
{
    private readonly LoginRepository loginRepository = new LoginRepository();
    const string InsSql = @"INSERT INTO Cliente (IdUsuario, Nombre, Apellido, DNI Telefono, Localidad, Edad) 
                            VALUES (@IdUsuario, @Nombre, @Apellido, @DNI, @Telefono, @Localidad, @Edad); 

                            SELECT LAST_INSERT_ID();";
    const string UpdSql = @"UPDATE Cliente 
                            SET Nombre = @Nombre, 
                                Apellido = @Apellido,
                                Email = @Email, 
                                Telefono = @Telefono,
                                Localidad = @Localidad
                            WHERE IdCliente = @IdCliente;";
    public IEnumerable<Cliente> SelectAll() => db.Query<Cliente>("SELECT * FROM Cliente");
    public Cliente? Select(int id) => db.QueryFirstOrDefault<Cliente>("SELECT * FROM Cliente WHERE IdCliente = @ID", new { ID = id });
    public Cliente Insert(Cliente cliente, Usuario usuario)
    {
        cliente.IdUsuario = loginRepository.Insert(usuario).IdUsuario;
        cliente.IdCliente = db.ExecuteScalar<int>(InsSql, cliente);
        return cliente;
    }
    public Cliente Update(Cliente cliente, int idCliente)
    {
        cliente.IdCliente = idCliente;
        db.Execute(UpdSql, cliente);
        return cliente;
    }

}
