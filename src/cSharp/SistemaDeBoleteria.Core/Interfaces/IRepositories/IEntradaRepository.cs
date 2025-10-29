using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IEntradaRepository
    {
        IEnumerable<Entrada> SelectAll();
        Entrada? Select(int IdEntrada);
        bool UpdateEstado(int IdEntrada);
    }
}