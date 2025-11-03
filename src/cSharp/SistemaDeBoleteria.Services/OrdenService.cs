using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Interfaces.IServices;
using SistemaDeBoleteria.Repositories;
using SistemaDeBoleteria.Core.DTOs;
using SistemaDeBoleteria.Core.Models;
using Mapster;

namespace SistemaDeBoleteria.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly OrdenRepository ordenRepository = new OrdenRepository();
        public IEnumerable<MostrarOrdenDTO> GetAll() => ordenRepository.SelectAll().Adapt<IEnumerable<MostrarOrdenDTO>>();
        public MostrarOrdenDTO? Get(int idOrden) => ordenRepository.Select(idOrden).Adapt<MostrarOrdenDTO>();
        public MostrarOrdenDTO Post(CrearOrdenDTO orden) => ordenRepository.Insert(orden.Adapt<Orden>()).Adapt<MostrarOrdenDTO>();
        public bool PagarOrden(int idOrden) => ordenRepository.UpdEstadoPagado(idOrden);
        public bool CancelarOrden(int idOrden) => ordenRepository.UpdEstadoCancelado(idOrden);
    }
}