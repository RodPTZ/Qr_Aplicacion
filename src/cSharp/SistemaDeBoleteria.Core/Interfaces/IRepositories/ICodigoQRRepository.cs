using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ICodigoQRRepository
    {
        CodigoQR? SelectById(int IdEntrada);
        CodigoQR.estadoQR UpdateEstado(int IdEntrada, CodigoQR.estadoQR estado);
        // dynamic SelectData(int idEntrada);
        // bool Exists(string codigo);
    }
}