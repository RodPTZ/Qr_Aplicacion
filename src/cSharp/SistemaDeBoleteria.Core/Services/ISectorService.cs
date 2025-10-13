namespace SistemaDeBoleteria.Core.Services;

public interface ISectorService
{
    void InsertSector(Sector sector, int idLocal);
    IEnumerable<Sector> GetSectores();
    Sector GetSectorById(int id);
    bool UpdateSector(Sector sector);
    bool DeleteSector(int id);
}
