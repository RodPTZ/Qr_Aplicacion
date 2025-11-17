using Mapster;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Exceptions;
using MySqlConnector;

namespace SistemaDeBoleteria.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository eventoRepository;
        private readonly ILocalRepository localRepository;
        public EventoService(IEventoRepository eventoRepository, ILocalRepository localRepository)
        {
            this.eventoRepository = eventoRepository;
            this.localRepository = localRepository;
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
                throw new NoContentException("El evento no puede publicarse: no tiene funciones");
            if(!eventoRepository.HasTarifasActivas(IdEvento))
                throw new NoContentException("El evento no puede publicarse: no tiene tarifas activas.");

            try
            {
                eventoRepository.UpdEstadoPublic(IdEvento);
            }
            catch (MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
        }
        
        public void CancelarEvento(int IdEvento)
        { 
            if(!eventoRepository.Exists(IdEvento))
                throw new NotFoundException("No se encontró el evento especificado");

            try
            {
                eventoRepository.UpdEstadoCancel(IdEvento);
            }
            catch(MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
        }
    }
}