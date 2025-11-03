using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IOrdenRepository
    {
        IEnumerable<Orden> SelectAll();
        Orden? Select(int idOrden);
        Orden Insert(Orden orden);
        bool UpdEstadoPagado(int idOrden);
        bool UpdEstadoCancelado(int idOrden);
    }
}