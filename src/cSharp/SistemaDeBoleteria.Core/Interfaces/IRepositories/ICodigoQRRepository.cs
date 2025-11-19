using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.Interfaces.IRepositories
{
    public interface ICodigoQRRepository
    {
        CodigoQR? SelectById(int IdEntrada);
        ETipoEstadoQR UpdateEstado(int IdEntrada, ETipoEstadoQR estado);
        bool UpdAYaUsada(int idEntrada);
        (Entrada, Funcion, CodigoQR) SelectData(int idEntrada);
        bool Exists(int idEntrada, string codigo);
        bool Insert(int idEntrada);
    }
}