using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
namespace SistemaDeBoleteria.Core.Interfaces.IServices;

public interface ISectorService
{
    IEnumerable<MostrarSectorDTO> GetAllByLocalId(int idLocal);
    MostrarSectorDTO Post(CrearActualizarSectorDTO sector, int idLocal);
    MostrarSectorDTO Put(CrearActualizarSectorDTO sector, int idSector);
    void Delete(int idSector);
    
}
