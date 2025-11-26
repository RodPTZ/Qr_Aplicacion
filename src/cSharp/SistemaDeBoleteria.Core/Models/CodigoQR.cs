using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Enums;
namespace SistemaDeBoleteria.Core.Models
{
    public class CodigoQR
    {
        public int IdQR { get; set; }
        public int IdEntrada { get; set; }
        public string Codigo  { get; set; }
        public ETipoEstadoQR TipoEstado  { get; set; }
        public CodigoQR(int idEntrada, string codigo)
        {
            IdEntrada = idEntrada;
            Codigo = codigo;
            TipoEstado = ETipoEstadoQR.NoExiste;
        }
        public CodigoQR()
        {   
        }

    }
}