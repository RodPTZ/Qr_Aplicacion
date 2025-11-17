using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;
using SistemaDeBoleteria.Core.Enums;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearTarifaDTO
    {
        public int IdFuncion { get; set; }
        public ETipoEntrada TipoEntrada { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
    public class ActualizarTarifaDTO
    {
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public ETipoEstadoTarifa Estado { get; set; }
    }
    public class MostrarTarifaDTO
    {
        public int IdTarifa { get; set; }
        public int IdFuncion { get; set; }
        public string TipoEntrada { get; set; }
        public decimal Precio { get; set; }
        public short Stock { get; set; }
        public string Estado { get; set; }
    }
}