using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IOrdenRepository
    {
        IEnumerable<Orden> SelectAll();
        Orden? Select(int idOrden);
        Orden Insert(Orden orden);
        bool UpdEstadoPagado(int idOrden, ETipoEntrada tipoEntrada);
        bool UpdEstadoCancelado(int idOrden);
        bool UpdEstadoExpirado(int idOrden);
        bool Exists(int idOrden);
        (ETipoEntrada, ETipoEstadoOrden, DateTime) Data(int unIdOrden);
    }
}