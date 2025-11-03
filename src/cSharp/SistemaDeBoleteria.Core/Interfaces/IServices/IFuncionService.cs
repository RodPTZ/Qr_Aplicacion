using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IFuncionService
{
    MostrarFuncionDTO Post(CrearFuncionDTO funcion);
    IEnumerable<MostrarFuncionDTO> GetAll();
    MostrarFuncionDTO? Get(int IdFuncion);
    MostrarFuncionDTO Put(ActualizarFuncionDTO funcion, int IdFuncion);
    void Cancelar(int idFuncion);
}
