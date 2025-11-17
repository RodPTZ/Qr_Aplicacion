using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using MySqlConnector;

namespace SistemaDeBoleteria.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository ordenRepository;
        private readonly ITarifaRepository tarifaRepository;
        private readonly IFuncionRepository funcionRepository;
        private readonly IClienteRepository clienteRepository;
        public OrdenService(IOrdenRepository ordenRepository, ITarifaRepository tarifaRepository, IFuncionRepository funcionRepository, IClienteRepository clienteRepository)
        {
            this.ordenRepository = ordenRepository;
            this.tarifaRepository = tarifaRepository;
            this.funcionRepository = funcionRepository;
            this.clienteRepository = clienteRepository;
        }
        public IEnumerable<MostrarOrdenDTO> GetAll()
        => ordenRepository
                .SelectAll()
                .Adapt<IEnumerable<MostrarOrdenDTO>>();
        public MostrarOrdenDTO? Get(int idOrden) 
        => ordenRepository
                .Select(idOrden)
                .Adapt<MostrarOrdenDTO>();
        public MostrarOrdenDTO Post(CrearOrdenDTO orden)
        {
            if(!tarifaRepository.Exists(orden.IdTarifa))
                throw new NotFoundException("No se encontró la tarifa especificada.");
            if(!funcionRepository.Exists(orden.IdFuncion))
                throw new NotFoundException("No se encontró la función especificada.");
            if(!clienteRepository.Exists(orden.IdCliente))
                throw new NotFoundException("No se encontró el cliente especificado.");

            Orden _orden;
            try
            {
                _orden = ordenRepository.Insert(orden.Adapt<Orden>());
            }
            catch(MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
            return _orden.Adapt<MostrarOrdenDTO>();
        }
        public bool PagarOrden(int idOrden)
        {
            var (EstadoOrden, CierreOrden) = ordenRepository.Data(idOrden);
            
            if(!ordenRepository.Exists(idOrden))
                throw new NotFoundException("No se encontró la orden especificada.");
            if(EstadoOrden == ETipoEstadoOrden.Cancelado)
                throw new BusinessException("No se puede pagar una orden que se encuentra Cancelada.");
            if(EstadoOrden == ETipoEstadoOrden.Abonado)
                throw new BusinessException("No se puede pagar una orden que se encuentra pagada.");
            if(CierreOrden < DateTime.Now)
            {   
                if(!ordenRepository.UpdEstadoExpirado(idOrden))
                    throw new DataBaseException("No se pudo actualizar la orden especificada.");
                throw new BusinessException($"Ya pasaron los 15 min hábiles para pagar la orden.");
            }

            try
            {
                return ordenRepository.UpdEstadoPagado(idOrden);
            }
            catch(MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
        }
        public bool CancelarOrden(int idOrden)
        {
            var (estadoOrden, _) = ordenRepository.Data(idOrden);
            
            if(!ordenRepository.Exists(idOrden))
                throw new NotFoundException("No se encontró la orden especificada.");
            if(estadoOrden == ETipoEstadoOrden.Cancelado)
                throw new BusinessException("No se puede cancelar una orden que se encuentra cancelada");

            try
            {
                return ordenRepository.UpdEstadoCancelado(idOrden);
            }
            catch(MySqlException ex)
            {
                throw new DataBaseException(ex.Message);
            }
        }
    }
}