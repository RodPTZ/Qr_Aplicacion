using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IOrdenService
{
    IEnumerable<MostrarOrdenDTO> GetAll();
    MostrarOrdenDTO? Get(int idOrden);
    MostrarOrdenDTO Post(CrearOrdenDTO orden);
    bool PagarOrden(int idOrden);
    bool CancelarOrden(int idOrden);
}
