using MySqlConnector;
using Dapper;
using System.Data;
using SistemaDeBoleteria.Core;
using SistemaDeBoleteria.Core.Services;

namespace SistemaDeBoleteria.AdoDapper;

public class ClienteAdo : IClienteService
{
    private readonly IDbConnection db;
    public ClienteAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public ClienteAdo(IDbConnection dbConnection) => db = dbConnection;
    public ClienteAdo()
    {
        db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }

    
    public IEnumerable<Cliente> GetClientes()
    {
        var sql = "SELECT * FROM Cliente";
        return db.Query<Cliente>(sql);
    }
    public Cliente? GetClienteById(int id)
    {
        var sql = "SELECT * FROM Cliente WHERE IdCliente = @ID";
        return db.QueryFirstOrDefault<Cliente>(sql, new { ID = id });
    }
    public void InsertCliente(Cliente cliente)
    {
        var sql = "INSERT INTO Cliente (Nombre, Apellido, DNI, Email, Telefono, Localidad, Edad) VALUES (@Nombre, @Apellido, @DNI, @Email, @Telefono, @Localidad, @Edad);";
        var id = db.ExecuteScalar<int>(sql, cliente);
        cliente.IdCliente = id;
    }
    public void UpdateCliente(Cliente cliente)
    {
        var sql = "UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, DNI = @DNI, Email = @Email, Telefono = @Telefono, Localidad = @Localidad WHERE IdCliente = @IdCliente";
        db.Execute(sql, cliente);
    }

}
