using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IOrdenService
{
    IEnumerable<MostrarOrdenDTO> GetOrdenes();
    MostrarOrdenDTO? GetOrdenById(int idOrden);
    MostrarOrdenDTO InsertOrden(CrearOrdenDTO orden);
    bool PagarOrden(int idOrden);
    bool CancelarOrden(int idOrden);
}
