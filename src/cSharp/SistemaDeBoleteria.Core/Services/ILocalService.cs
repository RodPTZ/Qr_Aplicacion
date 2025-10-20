using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Services;

public interface ILocalService
{
    IEnumerable<MostrarLocalDTO> GetLocales();
    MostrarLocalDTO? GetLocalById(int id);
    MostrarLocalDTO InsertLocal(CrearActualizarLocalDTO local);
    MostrarLocalDTO UpdateLocal(CrearActualizarLocalDTO local, int IdLocal);
    bool DeleteLocal(int id);
}
