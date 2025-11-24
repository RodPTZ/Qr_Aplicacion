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
        public string TipoEntrada { get; set; }
        public DateTime Emision { get; set; }
        public DateTime Liquidez { get; set; }
        public string Anulado { get; set; }
    }
}