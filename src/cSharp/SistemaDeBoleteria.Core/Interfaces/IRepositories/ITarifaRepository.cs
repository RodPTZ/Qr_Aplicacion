using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ITarifaRepository
    {
        IEnumerable<Tarifa> SelectAllByFuncionId(int idFuncion);
        Tarifa? Select(int idFuncion);
        Tarifa Insert(Tarifa tarifa);
        bool Update(Tarifa tarifa, int IdTarifa);
        bool Exists(int idTarifa);
        bool ReducirStock( int idTarifa);
        bool DevolverStock(int idOrden);
        bool SuspenderTarifasPorIdEvento(int idEvento);
        bool SuspenderTarifasPorIdFuncion(int idFuncion);
    }
}