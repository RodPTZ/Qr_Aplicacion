using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IClienteService
{
    IEnumerable<MostrarClienteDTO> GetAll();
    MostrarClienteDTO? GetById(int id);
    MostrarClienteDTO Post(CrearClienteDTO cliente);
    MostrarClienteDTO? Put(ActualizarClienteDTO cliente, int idCliente);
}
