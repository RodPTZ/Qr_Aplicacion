using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Exceptions;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeBoleteria.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository eventoRepository;
        private readonly ILocalRepository localRepository;
        private readonly IFuncionRepository funcionRepository;
        private readonly IEntradaRepository entradaRepository;
        private readonly ITarifaRepository tarifaRepository;
        public EventoService(IEventoRepository eventoRepository, ILocalRepository localRepository, IFuncionRepository funcionRepository, IEntradaRepository entradaRepository, ITarifaRepository tarifaRepository)
        {
            this.eventoRepository = eventoRepository;
            this.localRepository = localRepository;
            this.funcionRepository = funcionRepository;
            this.entradaRepository = entradaRepository;
            this.tarifaRepository = tarifaRepository;
        }
        public IEnumerable<MostrarEventoDTO> GetAll()
        => eventoRepository
                    .SelectAll()
                    .Adapt<IEnumerable<MostrarEventoDTO>>();

        public MostrarEventoDTO? GetById(int IdEvento)
        => eventoRepository
                    .Select(IdEvento)
                    .Adapt<MostrarEventoDTO>();
                    
        public MostrarEventoDTO Post(CrearActualizarEventoDTO evento)
        {
            if(!localRepository.Exists(evento.IdLocal))
                throw new NotFoundException("No se encontró el local especificado.");
            
            return eventoRepository
                    .Insert(evento.Adapt<Evento>())
                    .Adapt<MostrarEventoDTO>();
        }
                    
        public MostrarEventoDTO? Put(CrearActualizarEventoDTO evento, int IdEvento) 
        {
            if(!eventoRepository.Exists(IdEvento))
                throw new NotFoundException("No se encontró el evento especificado para actualizar.");
    
            if(!eventoRepository.Update(evento.Adapt<Evento>(), IdEvento))
                throw new BusinessException("No se pudo actualizar el evento");

            return eventoRepository.Select(IdEvento).Adapt<MostrarEventoDTO>();
        }
        
        public void PublicarEvento(int IdEvento)
        {
            if(!eventoRepository.Exists(IdEvento))
                throw new NotFoundException("No se encontró el evento especificado.");
            if(!eventoRepository.HasFunciones(IdEvento))
                throw new BusinessException("El evento no puede publicarse: no tiene funciones");
            if(!eventoRepository.HasTarifasActivas(IdEvento))
                throw new BusinessException("El evento no puede publicarse: no tiene tarifas activas.");

            // debería ser atómico
            if(!eventoRepository.UpdPublicado(IdEvento))
                throw new DataBaseException("No se pudo publicar el evento.");
            if(!funcionRepository.UpdPublicado(IdEvento))
                throw new DataBaseException("No se pudo publicar la función");
            
            // try
            // {
            //     eventoRepository.UpdEstadoPublic(IdEvento);
            // }
            // catch (MySqlException ex)
            // {
            //     throw new DataBaseException(ex.Message);
            // }
        }
        
        public void CancelarEvento(int IdEvento)
        { 
            if(!eventoRepository.Exists(IdEvento))
                throw new NotFoundException("No se encontró el evento especificado");

            if(!entradaRepository.UpdAnularEntradasDeEventoID(IdEvento))
                throw new DataBaseException("No se pudieron anular las entradas relacionadas al evento");
            if(!tarifaRepository.SuspenderTarifasPorIdEvento(IdEvento))
                throw new DataBaseException("No se pudieron suspender y devolver stock a las tarifas relacionadas al evento.");
            if(!funcionRepository.UpdCancelarFuncionesDeIdEvento(IdEvento))
                throw new DataBaseException("No se pudieron cancelar las funciones relacionadas al evento");
            if(!eventoRepository.UpdCancelar(IdEvento))
                throw new DataBaseException("No se pudo cancelar el evento");
            // try
            // {
            //     eventoRepository.UpdEstadoCancel(IdEvento);
            // }
            // catch(MySqlException ex)
            // {
            //     throw new DataBaseException(ex.Message);
            // }
        }
    }
}