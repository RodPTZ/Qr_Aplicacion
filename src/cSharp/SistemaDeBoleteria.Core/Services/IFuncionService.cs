using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Services;

public interface IFuncionService
{
    MostrarFuncionDTO InsertFuncion(CrearFuncionDTO funcion);
    IEnumerable<MostrarFuncionDTO> GetFunciones();
    MostrarFuncionDTO? GetFuncionById(int IdFuncion);
    MostrarFuncionDTO UpdateFuncion(ActualizarFuncionDTO funcion, int IdFuncion);
    void CancelarFuncion(int IdFuncion);
}
