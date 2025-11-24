using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using SistemaDeBoleteria.Repositories;

namespace SistemaDeBoleteria.Services
{
    public class FuncionService : IFuncionService
    {
        private readonly IFuncionRepository funcionRepository;
        private readonly IEventoRepository eventoRepository;
        private readonly ISectorRepository sectorRepository;
        private readonly IEntradaRepository entradaRepository;
        private readonly ITarifaRepository tarifaRepository;
        public FuncionService(IFuncionRepository funcionRepository, IEventoRepository eventoRepository, ISectorRepository sectorRepository, IEntradaRepository entradaRepository, ITarifaRepository tarifaRepository)
        {
            this.funcionRepository = funcionRepository;
            this.eventoRepository = eventoRepository;
            this.sectorRepository = sectorRepository;
            this.entradaRepository = entradaRepository;
            this.tarifaRepository = tarifaRepository;
        }

        public IEnumerable<MostrarFuncionDTO> GetAll() 
        => funcionRepository
                .SelectAll()
                .Adapt<IEnumerable<MostrarFuncionDTO>>();

        public MostrarFuncionDTO? Get(int idFuncion) 
        => funcionRepository
                .Select(idFuncion)
                .Adapt<MostrarFuncionDTO>();
        public MostrarFuncionDTO Post(CrearFuncionDTO funcion)
        { 
            if(!eventoRepository.Exists(funcion.IdEvento))
                throw new NotFoundException("No se encontró el evento especificado.");
            if(!sectorRepository.Exists(funcion.IdSector))
                throw new NotFoundException("No se encontró el sector especificado.");
            
            return funcionRepository.Insert(funcion.Adapt<Funcion>()).Adapt<MostrarFuncionDTO>();
        }
        public MostrarFuncionDTO Put(ActualizarFuncionDTO funcion, int idFuncion)
        {
            if(!funcionRepository.Exists(idFuncion))
                throw new NotFoundException("No se encontró la función especificada.");
            if(!sectorRepository.Exists(funcion.IdSector))
                throw new NotFoundException("No se encontró el sector especificado.");
            if(!funcionRepository.Update(funcion.Adapt<Funcion>(), idFuncion))
                throw new BusinessException("No se pudo actualizar la función especificada");

            return funcionRepository.Select(idFuncion).Adapt<MostrarFuncionDTO>();
        }
        public void Cancelar(int idFuncion)
        { 
            if(!funcionRepository.Exists(idFuncion))
                throw new NotFoundException("No se encontró la función especificada");
            if(!entradaRepository.UpdAnularEntradasDeFuncionID(idFuncion))
                throw new DataBaseException("No se pudieron anular las entradas relacionadas a la función especificada.");
            if(!tarifaRepository.SuspenderTarifasPorIdFuncion(idFuncion))
                throw new DataBaseException("No se pudieron suspender y devolver stock a las tarifas relacionadas a la función especificada.");
            if(!funcionRepository.UpdFuncionCancel(idFuncion))
                throw new DataBaseException("No se pudo cancelar la función especificada");
                
        }
    }
}