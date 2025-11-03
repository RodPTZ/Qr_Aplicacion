using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ITarifaService
{
    MostrarTarifaDTO Post(CrearTarifaDTO tarifa);
    IEnumerable<MostrarTarifaDTO> GetAllByFuncionId(int IdFuncion);
    MostrarTarifaDTO? Get(int id);
    MostrarTarifaDTO Put(ActualizarTarifaDTO tarifa, int IdTarifa);
}
