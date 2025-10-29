using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ISectorService
{
    IEnumerable<MostrarSectorDTO> GetSectoresByLocalId(int idLocal);
    MostrarSectorDTO InsertSector(CrearActualizarSectorDTO sector, int idLocal);
    MostrarSectorDTO UpdateSector(CrearActualizarSectorDTO sector, int idSector);
    bool DeleteSector(int idSector);
    
}
