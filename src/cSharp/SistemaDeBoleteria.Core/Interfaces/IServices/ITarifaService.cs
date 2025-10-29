using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ITarifaService
{
    MostrarTarifaDTO InsertTarifa(CrearTarifaDTO tarifa);
    IEnumerable<MostrarTarifaDTO> GetTarifasByFuncionId(int IdFuncion);
    MostrarTarifaDTO? GetTarifaById(int id);
    MostrarTarifaDTO UpdateTarifa(ActualizarTarifaDTO tarifa, int IdTarifa);
}
