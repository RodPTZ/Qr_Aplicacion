using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class MostrarEntradaDTO
    {
        public int IdEntrada { get; set; }
        public int IdOrden { get; set; }
        public Orden.TipoEntrada Categoria { get; set; }
        public DateTime FechaLiquidez { get; set; }
        public DateTime FechaEmisión { get; set; }
        public bool Anulada { get; set; }
    }
}