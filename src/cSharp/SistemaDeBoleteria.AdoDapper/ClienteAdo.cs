using MySqlConnector;
using Dapper;
using System.Data;
using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Services;
using SistemaDeBoleteria.Core.DTOs;

namespace SistemaDeBoleteria.AdoDapper;

public class ClienteAdo : IClienteService
{
    private readonly IDbConnection db;
    public ClienteAdo(string connectionString) => db = new MySqlConnection(connectionString);
    public ClienteAdo(IDbConnection dbConnection) => db = dbConnection;
    public ClienteAdo()
    {
        // db = new MySqlConnection($"Server=localhost;Database=bd_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
        db = new MySqlConnection($"Server=localhost;Database=5to_SistemaDeBoleteria;uid=5to_agbd;Password=Trigg3rs!");
    }
    
    public IEnumerable<MostrarClienteDTO> GetClientes()
    {
        var sql = "SELECT * FROM Cliente";
        return db.Query<MostrarClienteDTO>(sql);
    }
    public MostrarClienteDTO? GetClienteById(int id)
    {
        var sql = "SELECT * FROM Cliente WHERE IdCliente = @ID";
        return db.QueryFirstOrDefault<MostrarClienteDTO>(sql, new { ID = id });
    }
    public MostrarClienteDTO? InsertCliente(CrearClienteDTO crearCliente)
    {
        var cliente = crearCliente.Adapt<Cliente>();
        var sql = "INSERT INTO Cliente (Nombre, Apellido, DNI, Email, Telefono, Localidad, Edad, Contraseña) VALUES (@Nombre, @Apellido, @DNI, @Email, @Telefono, @Localidad, @Edad, @Contraseña); SELECT LAST_INSERT_ID();";
        var id = db.ExecuteScalar<int>(sql, cliente);
        cliente.IdCliente = id;
        return cliente.Adapt<MostrarClienteDTO>();

    }
    public MostrarClienteDTO? UpdateCliente(ActualizarClienteDTO cliente, int idCliente)
    {
        var clienteAdaptado = cliente.Adapt<Cliente>();
        clienteAdaptado.IdCliente = idCliente;
        var sql = "UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Telefono = @Telefono, Localidad = @Localidad WHERE IdCliente = @IdCliente;";
        db.Execute(sql, clienteAdaptado);

        return GetClienteById(clienteAdaptado.IdCliente);
    }

}
