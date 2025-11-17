using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Services
{
    public class SectorService : ISectorService
    {
        private readonly ISectorRepository sectorRepository ;
        private readonly ILocalRepository localRepository;
        public SectorService(ISectorRepository sectorRepository, ILocalRepository localRepository)
        {
            this.sectorRepository = sectorRepository;
            this.localRepository = localRepository;
        }
        public IEnumerable<MostrarSectorDTO> GetAllByLocalId(int idLocal) 
        => sectorRepository
                    .SelectAllByLocalId(idLocal)
                    .Adapt<IEnumerable<MostrarSectorDTO>>();
        public MostrarSectorDTO Post(CrearActualizarSectorDTO sector, int idLocal)
        {
            if(!localRepository.Exists(idLocal))
                throw new NotFoundException("No se encontró el local especificado para asignar el sector.");

            return sectorRepository
                    .Insert(sector.Adapt<Sector>(), idLocal)
                    .Adapt<MostrarSectorDTO>();
        }
        public MostrarSectorDTO Put(CrearActualizarSectorDTO sector, int idSector)
        {
            if(!sectorRepository.Exists(idSector))
                throw new NotFoundException("No se encontró el sector especificado para actualizar.");
            if(!sectorRepository.Update(sector.Adapt<Sector>(), idSector))
                throw new BusinessException("No se pudo actualizar el sector especificado.");

            return sectorRepository
                    .Select(idSector)
                    .Adapt<MostrarSectorDTO>();
        }
        public void Delete(int idSector)
        { 
            if(!sectorRepository.Exists(idSector))
                throw new NotFoundException("No se encontró el sector especificado para eliminar.");
            if (sectorRepository.HasFunciones(idSector))
                throw new BusinessException("No se puede eliminar el sector porque tiene funciones asociadas.");
            if(!sectorRepository.Delete(idSector))
                throw new NotFoundException("No se encontró el sector especificado, o ya fue eliminado.");
        }
    }
}