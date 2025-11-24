using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Core.Interfaces.IRepositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Exceptions;
using Mapster;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeBoleteria.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository ordenRepository;
        private readonly ITarifaRepository tarifaRepository;
        private readonly IClienteRepository clienteRepository;
        private readonly IEntradaRepository entradaRepository;
        private readonly ICodigoQRRepository codigoQRRepository;
        public OrdenService(IOrdenRepository ordenRepository, ITarifaRepository tarifaRepository, IClienteRepository clienteRepository, IEntradaRepository entradaRepository, ICodigoQRRepository codigoQRRepository)
        {
            this.ordenRepository = ordenRepository;
            this.tarifaRepository = tarifaRepository;
            this.clienteRepository = clienteRepository;
            this.entradaRepository = entradaRepository;
            this.codigoQRRepository = codigoQRRepository;
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
            if(!clienteRepository.Exists(orden.IdCliente))
                throw new NotFoundException("No se encontró el cliente especificado.");

            //deberia tener trycatch
            var newOrden = ordenRepository.Insert(orden.Adapt<Orden>());
            if(newOrden is null)
                throw new DataBaseException("No se pudo instanciar la orden");
            if(!tarifaRepository.ReducirStock( newOrden.IdTarifa ))
                throw new DataBaseException("No se pudo reducir el stock.");

            return newOrden.Adapt<MostrarOrdenDTO>();
        }
        public bool PagarOrden(int idOrden)
        {
            if(!ordenRepository.Exists(idOrden))
                throw new NotFoundException("No se encontró la orden especificada.");
            
            var (TipoEntrada, EstadoOrden, CierreOrden, Cancelado, Stock, EstadoTarifa, EstadoEvento) = ordenRepository.Data(idOrden);

            if(EstadoOrden == ETipoEstadoOrden.Cancelado)
                throw new BusinessException("No se puede pagar una orden que se encuentra Cancelada.");
            if(EstadoOrden == ETipoEstadoOrden.Abonado)
                throw new BusinessException("No se puede pagar una orden que se encuentra pagada.");
            if(Cancelado is true)
                throw new BusinessException("La función fue cancelada. No se puede comprar la entrada.");
            if(Stock <= 0)
                throw new BusinessException("No hay más stock disponible para esta tarifa.");
            if(EstadoTarifa != ETipoEstadoTarifa.Activa)
                throw new BusinessException("La tarifa no está activa. No se puede comprar la entrada.");
            if(EstadoEvento != ETipoEstadoEvento.Publicado)
                throw new BusinessException("El evento aún no está publicado. No se puede comprar");
            if(CierreOrden < DateTime.Now.ToLocalTime())
            {   
                if(!ordenRepository.UpdEstadoExpirado(idOrden))
                    throw new DataBaseException("No se pudo actualizar la orden especificada.");
                throw new BusinessException($"Ya pasaron los 15 min hábiles para pagar la orden.");
            }

            if(!ordenRepository.UpdAbonado(idOrden))
                throw new DataBaseException("No se pudo abonar la orden");

            var idEntrada = entradaRepository.Insert(idOrden, TipoEntrada);
            if(idEntrada == 0)
                throw new DataBaseException("No se pudo instanciar la entrada");

            if(!codigoQRRepository.Insert(idEntrada))
                throw new DataBaseException("No se pudo instanciar el código QR");
            return true;
        }
        public bool CancelarOrden(int idOrden)
        {
            var (_, estadoOrden, _, _ , _, _, _) = ordenRepository.Data(idOrden);
            
            if(!ordenRepository.Exists(idOrden))
                throw new NotFoundException("No se encontró la orden especificada.");
            if(estadoOrden == ETipoEstadoOrden.Cancelado)
                throw new BusinessException("No se puede cancelar una orden que se encuentra cancelada");

            //deberia ir trycatch
            if(!tarifaRepository.DevolverStock(idOrden))
                throw new DataBaseException("No se pudo devolver el stock a la tarifa");
            if(!ordenRepository.UpdCancelado(idOrden))
                throw new DataBaseException("No se pudo cancelar la orden");
            return true;
            
            // try
            // {
            //     return ordenRepository.UpdEstadoCancelado(idOrden);
            // }
            // catch(MySqlException ex)
            // {
            //     throw new DataBaseException(ex.Message);
            // }
        }
    }
}