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
        string? UpdateEstado(int IdEntrada, string estado);
    }
}