using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IEventoRepository
    {
        IEnumerable<Evento> SelectAll();
        Evento? Select(int IdEvento);
        Evento Insert(Evento evento);
        bool Update(Evento evento, int IdEvento);
        bool Exists(int IdEvento);
        bool HasFunciones(int idEvento);
        bool HasTarifasActivas(int idEvento);
        bool UpdPublicado(int idEvento);
        bool UpdCancelar(int idEvento);
    }
}