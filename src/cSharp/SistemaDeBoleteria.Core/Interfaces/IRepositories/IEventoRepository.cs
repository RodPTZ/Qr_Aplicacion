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
        Evento Update(Evento evento, int IdEvento);
        bool PublicarEvento(int IdEvento);
        bool CancelarEvento(int IdEvento);
    }
}