using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Services;

public interface IEntradaService
{
    IEnumerable<MostrarEntradaDTO> GetEntradas();
    MostrarEntradaDTO? GetEntradaById(int id);
    void AnularEntrada(int id);
}
