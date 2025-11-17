using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ILocalService
{
    IEnumerable<MostrarLocalDTO> GetAll();
    MostrarLocalDTO? Get(int id);
    MostrarLocalDTO Post(CrearActualizarLocalDTO local);
    MostrarLocalDTO Put(CrearActualizarLocalDTO local, int IdLocal);
    void Delete(int id);
}
