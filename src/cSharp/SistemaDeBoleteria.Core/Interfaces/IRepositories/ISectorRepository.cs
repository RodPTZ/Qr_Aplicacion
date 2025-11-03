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
        Sector Insert(Sector sector, int idLocal);
        Sector Update(Sector sector, int idSector);
        bool Delete(int idSector);
    }
}