using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface IFuncionRepository
    {
        Funcion Insert(Funcion funcion);
        IEnumerable<Funcion> SelectAll();
        Funcion? Select(int IdFuncion);
        Funcion Update(Funcion funcion, int idFuncion);
        void UpdFuncionCancel(int idFuncion);
    }
}