using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ILocalRepository
    {
        IEnumerable<Local> SelectAll();
        Local? Select(int idLocal);
        Local Insert(Local local);
        Local Update(Local local, int IdLocal);
        bool Delete(int IdLocal);
    }
}