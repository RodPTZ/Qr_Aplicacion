using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface IEntradaService
{
    IEnumerable<MostrarEntradaDTO> GetAll();
    MostrarEntradaDTO? GetById(int id);
    bool AnularEntrada(int id);
}
