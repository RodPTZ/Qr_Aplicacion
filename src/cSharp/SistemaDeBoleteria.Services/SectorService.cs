using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Repositories;
using Mapster;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Services
{
    public class SectorService : ISectorService
    {
        private readonly SectorRepository sectorRepository = new SectorRepository();
        public IEnumerable<MostrarSectorDTO> GetAllByLocalId(int idLocal) => sectorRepository.SelectAllByLocalId(idLocal).Adapt<IEnumerable<MostrarSectorDTO>>();
        public MostrarSectorDTO Post(CrearActualizarSectorDTO sector, int idLocal) => sectorRepository.Insert(sector.Adapt<Sector>(), idLocal).Adapt<MostrarSectorDTO>();
        public MostrarSectorDTO Put(CrearActualizarSectorDTO sector, int idSector) => sectorRepository.Update(sector.Adapt<Sector>(), idSector).Adapt<MostrarSectorDTO>();
        public bool Delete(int idSector) => sectorRepository.Delete(idSector);
    }
}