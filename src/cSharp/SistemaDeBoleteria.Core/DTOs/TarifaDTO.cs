using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SistemaDeBoleteria.Core.Models;

namespace SistemaDeBoleteria.Core.DTOs
{
    public class CrearTarifaDTO
    {
        public int IdFuncion { get; set; }
        public decimal precio { get; set; }
        public short stock { get; set; }
        public Tarifa.TipoEntrada entrada { get; set; }
    }
    public class ActualizarTarifaDTO
    {
        public decimal precio { get; set; }
        public short stock { get; set; }
        public Tarifa.TipoEstado estado { get; set; }
    }
    public class MostrarTarifaDTO
    {
        public int IdTarifa { get; set; }
        public int IdFuncion { get; set; }
        public decimal precio { get; set; }
        public short stock { get; set; }
        public Tarifa.TipoEstado estado { get; set; }
        public Tarifa.TipoEntrada entrada { get; set; }
    }
}