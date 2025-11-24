using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IEntradaRepository
    {
        IEnumerable<Entrada> SelectAll();
        Entrada? Select(int IdEntrada);
        bool UpdAnular(int idEntrada);
        int Insert(int idOrden, ETipoEntrada tipoDeEntrada);
        bool Exists(int idEntrada);
        bool UpdAnularEntradasDeEventoID(int idEvento);
        bool UpdAnularEntradasDeFuncionID(int idFuncion);
    }
}