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
        bool UpdEstadoExpirado(int idOrden);
        bool UpdAbonado(int idOrden);
        bool UpdCancelado(int idOrden);
        bool Exists(int idOrden);
        (ETipoEntrada, ETipoEstadoOrden, DateTime, bool, int, ETipoEstadoTarifa, ETipoEstadoEvento) Data(int unIdOrden);
    }
}