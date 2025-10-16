namespace SistemaDeBoleteria.Core.Services;

public interface ISectorService
{
    void InsertSector(Sector sector, int idLocal);
    IEnumerable<Sector> GetSectoresByLocalId(int idLocal);
    Sector? GetSectorByLocalId(int idLocal);
    bool UpdateSector(Sector sector, int id);
    bool DeleteSector(int id);
    Sector? GetSectorById(int idSector);
}
