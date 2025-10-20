using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Services;

public interface IClienteService
{
    IEnumerable<MostrarClienteDTO> GetClientes();
    MostrarClienteDTO? GetClienteById(int id);
    MostrarClienteDTO? InsertCliente(CrearClienteDTO cliente);
    MostrarClienteDTO? UpdateCliente(ActualizarClienteDTO cliente, int idCliente);
}
