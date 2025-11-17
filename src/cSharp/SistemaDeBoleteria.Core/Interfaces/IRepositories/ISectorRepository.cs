using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ISectorRepository
    {
        IEnumerable<Sector> SelectAllByLocalId(int idLocal);
        Sector? Select(int idSector);
        Sector Insert(Sector sector, int idLocal);
        bool Update(Sector sector, int idSector);
        bool Delete(int idSector);
        bool HasFunciones(int idSector);
        bool Exists(int idSector);
    }
}